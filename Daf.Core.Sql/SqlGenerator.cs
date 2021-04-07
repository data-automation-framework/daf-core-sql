// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Daf.Core.Sql.IonStructure;
using Daf.Core.Sql.Output;
using Daf.Core.Sql.Output.DatabaseObjects;
using Daf.Core.Sql.Output.DatabaseObjects.Components;
using Daf.Core.Sdk;

namespace Daf.Core.Sql
{
	public static class SqlGenerator
	{
		// TODO: Re-think how these properties are handled, avoid global state.
		public static Dictionary<string, Output.Database> Databases { get; } = new();

		public static Dictionary<string, string> IonNugets { get; } = new();

		public static void GenerateEverything(SqlProject project)
		{
			if (project == null)
				throw new ArgumentNullException(nameof(project));

			PopulateIonNugets();
			CreateDatabaseSolution(project, Properties.Instance.OutputDirectory!);
		}

		private static void CopyFolder(string absoluteSourcePath, string absoluteDestinationPath)
		{
			foreach (string filePath in Directory.GetFiles(absoluteSourcePath, "*.*", SearchOption.AllDirectories))
			{
				string relativeFilePath = filePath.Remove(0, absoluteSourcePath.Length + 1); // Remove root directory.
				string targetFilePath = Path.Combine(absoluteDestinationPath, relativeFilePath);

				Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath)!);
				File.Copy(filePath, targetFilePath, true);
			}
		}

		private static void CreateDatabaseSolution(SqlProject project, string outputPath)
		{
			// Create database objects.
			foreach (IonStructure.Database ionDatabase in project.Databases)
			{
				Databases[ionDatabase.Name] = new Output.Database(ionDatabase);
				Databases[ionDatabase.Name].GenerateObjects(ionDatabase);
			}

			string sqlSolutionOutputPath = Path.Combine(outputPath, project.Name);

			// Create database projects.
			List<Project> projects = CreateDatabaseProjects(sqlSolutionOutputPath);

			// Create solution.
			if (projects.Count != 0)
				Solution.Generate(project.Name, projects, sqlSolutionOutputPath);
		}

		private static List<Project> CreateDatabaseProjects(string sqlSolutionOutputPath)
		{
			List<Project> projects = new();

			foreach (Output.Database database in Databases.Values)
			{
				if (database.CreateProject)
				{
					Project project = new(database, projects);
					project.CreateProjectFiles(sqlSolutionOutputPath);

					// Copy included folders.
					foreach (string path in database.FolderIncludePaths)
					{
						string absoluteSourceFolderPath;
						if (path.StartsWith(@"Nuget\", StringComparison.Ordinal))
						{
							string[] pathParts = path.Split(@"\");
							absoluteSourceFolderPath = IonNugets[pathParts[1]] + path.Replace(@$"{pathParts[0]}\{pathParts[1]}", string.Empty, StringComparison.Ordinal);
						}
						else
							absoluteSourceFolderPath = Path.Combine(Properties.Instance.ProjectDirectory!, path);

						string absoluteDestinationPath = Path.Combine(sqlSolutionOutputPath, database.Name);

						if (Directory.Exists(absoluteSourceFolderPath))
							CopyFolder(absoluteSourceFolderPath, absoluteDestinationPath);
						else
							throw new DirectoryNotFoundException($"Failed to find database include folder {absoluteSourceFolderPath}!");
					}

					// Copy dacpac references.
					foreach (Output.DatabaseDependency databaseDependency in database.DatabaseDependencies)
					{
						if (databaseDependency.Type == DatabaseDependencyType.Dacpac)
						{
							string absoluteSourcePath = Path.Combine(Properties.Instance.ProjectDirectory!, databaseDependency.Reference);
							string dacpacFileName = Path.GetFileName(databaseDependency.Reference);
							string absoluteDestinationPath = Path.Combine(sqlSolutionOutputPath, dacpacFileName);

							if (!File.Exists(absoluteSourcePath))
								throw new FileNotFoundException($"Failed to find dacpac referenced by database {database.Name}!", dacpacFileName);

							if (!File.Exists(absoluteDestinationPath))
								File.Copy(absoluteSourcePath, absoluteDestinationPath);
						}
					}

					CreateDatabaseObjectFiles(database, sqlSolutionOutputPath);

					projects.Add(project);
				}
			}

			return projects;
		}

		private static void CreateDatabaseObjectFiles(Output.Database database, string outputPath)
		{
			string databasePath = Path.Combine(outputPath, database.Name);

			foreach (Output.DatabaseObjects.Schema schema in database.Schemas.Values)
			{
				// Create schema folders and files.
				string schemaOutputPath = Path.Combine(databasePath, schema.Path);
				Directory.CreateDirectory(Path.GetDirectoryName(schemaOutputPath)!);
				File.WriteAllText(schemaOutputPath, schema.ToString());

				// Create database objects, including the required folders.
				foreach (KeyValuePair<string, DatabaseObject> databaseObject in schema.DatabaseObjects)
				{
					string databaseObjectOutputPath = Path.Combine(databasePath, databaseObject.Value.Path);
					Directory.CreateDirectory(Path.GetDirectoryName(databaseObjectOutputPath)!);
					File.WriteAllText(databaseObjectOutputPath, databaseObject.Value.ToString());
				}
			}
		}

		public static Column GetReferencedColumn(string databaseName, IonStructure.ForeignKey foreignKeyColumn)
		{
			if (foreignKeyColumn == null)
				throw new ArgumentNullException(nameof(foreignKeyColumn));

			if (Databases.TryGetValue(databaseName, out Output.Database? database))
			{
				if (database.Schemas.TryGetValue(foreignKeyColumn.SchemaName, out Output.DatabaseObjects.Schema? schema))
				{
					if (schema.DatabaseObjects.TryGetValue(foreignKeyColumn.TableName, out DatabaseObject? table))
					{
						if (((Output.DatabaseObjects.Table)table).Columns.TryGetValue(foreignKeyColumn.ColumnName, out Column? column))
							return column;
					}
				}
			}

			throw new InvalidOperationException($"Failed to find column {foreignKeyColumn.ColumnName} in table {databaseName}.{foreignKeyColumn.SchemaName}.{foreignKeyColumn.TableName}!");
		}

		/// <summary>
		/// This is a huge mess.
		/// TODO: See if we can use Microsoft's Nuget libraries to do this instead.
		/// </summary>
		private static void PopulateIonNugets()
		{
			// Find .dafproj files in project directory. This will break if we ever support multiple .dafprojs in one directory.
			string projectFilePath = Directory.GetFiles(Properties.Instance.ProjectDirectory!, "*.dafproj")[0];

			string[] projectFileLines = File.ReadAllLines(projectFilePath);

			foreach (string line in projectFileLines)
			{
				if (line.Contains("PackageReference", StringComparison.Ordinal))
				{
					string packageName = Utility.FindStringBetweenStrings(line, "Include=\"", "\"");

					string libraryPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\.nuget\packages\{packageName.ToLower(CultureInfo.InvariantCulture)}";

					List<string> versionList = Directory.GetDirectories(libraryPath, "*").Select(x => new DirectoryInfo(x).Name).ToList();
					versionList.Sort((a, b) => string.Compare(b, a, StringComparison.InvariantCulture)); // Descending sort.

					string projectVersion = Utility.FindStringBetweenStrings(line, "Version=\"", "\"");

					// Attempt to figure out which version to match against, if wildcard matching is used.
					if (projectVersion.Contains('*', StringComparison.Ordinal))
					{
						foreach (string version in versionList)
						{
							if (version.Contains(projectVersion.Replace("*", string.Empty, StringComparison.Ordinal), StringComparison.Ordinal))
							{
								projectVersion = version;
								break;
							}
						}
					}

					// Figure out if this is a content-only library.
					bool contentOnlyLibrary = Directory.Exists($@"{libraryPath}\{projectVersion}\contentFiles") && !Directory.Exists($@"{libraryPath}\{projectVersion}\lib");

					if (contentOnlyLibrary)
						IonNugets.Add(packageName, $@"{libraryPath}\{projectVersion}\contentFiles\any\any");
				}
			}
		}
	}
}
