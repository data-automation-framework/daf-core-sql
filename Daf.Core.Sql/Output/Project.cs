// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Daf.Core.Sdk;
using Daf.Core.Sql.Output.DatabaseObjects;

namespace Daf.Core.Sql.Output
{
	public class Project
	{
		public Project(Database database, List<Project> previouslyCreatedProjects)
		{
			if (database == null)
				throw new ArgumentNullException(nameof(database));

			Name = database.Name;
			Database = database;
			Guid = System.Guid.NewGuid().ToString("B").ToUpper(CultureInfo.InvariantCulture);
			PreviouslyCreatedProjects = previouslyCreatedProjects;
		}

		public Database Database { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "It's the name used by MSBuild, so we're using it.")]
		public string Guid { get; }

		public string Name { get; }

		public List<Project> PreviouslyCreatedProjects { get; }

		public void CreateProjectFiles(string sqlSolutionOutputPath)
		{
			CreateSqlProjectFile(sqlSolutionOutputPath);

			if (Database.SchemaCompare != null)
				Database.SchemaCompare.CreateScmpFile(Database.Name, sqlSolutionOutputPath, Guid);
		}

		private void CreateSqlProjectFile(string sqlSolutionOutputPath)
		{
			StringBuilder stringBuilder = new();
			stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			stringBuilder.AppendLine("<Project DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\" ToolsVersion=\"4.0\">");
			stringBuilder.AppendLine("  <Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')\" />");
			stringBuilder.AppendLine("  <PropertyGroup>");
			stringBuilder.AppendLine("    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>");
			stringBuilder.AppendLine("    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>");
			stringBuilder.AppendLine($"    <Name>{Database.Name}</Name>");
			stringBuilder.AppendLine("    <SchemaVersion>2.0</SchemaVersion>");
			stringBuilder.AppendLine("    <ProjectVersion>4.1</ProjectVersion>");
			stringBuilder.AppendLine($"    <ProjectGuid>{Guid}</ProjectGuid>");

			if (Database.SqlServerPlatform == SqlServerPlatform.Azure)
				stringBuilder.AppendLine($"    <DSP>Microsoft.Data.Tools.Schema.Sql.SqlAzureV12DatabaseSchemaProvider</DSP>");
			else
				stringBuilder.AppendLine($"    <DSP>Microsoft.Data.Tools.Schema.Sql.Sql{(int)Database.SqlServerVersion}0DatabaseSchemaProvider</DSP>");

			stringBuilder.AppendLine("    <OutputType>Database</OutputType>");
			stringBuilder.AppendLine("    <RootPath>");
			stringBuilder.AppendLine("    </RootPath>");
			stringBuilder.AppendLine($"    <RootNamespace>{Database.Name}</RootNamespace>");
			stringBuilder.AppendLine($"    <AssemblyName>{Database.Name}</AssemblyName>");
			stringBuilder.AppendLine("    <ModelCollation>1033, CI</ModelCollation>");
			stringBuilder.AppendLine("    <DefaultFileStructure>BySchemaAndSchemaType</DefaultFileStructure>");
			stringBuilder.AppendLine("    <DeployToDatabase>True</DeployToDatabase>");
			stringBuilder.AppendLine("    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>");
			stringBuilder.AppendLine("    <TargetLanguage>CS</TargetLanguage>");
			stringBuilder.AppendLine("    <AppDesignerFolder>Properties</AppDesignerFolder>");
			stringBuilder.AppendLine("    <SqlServerVerification>False</SqlServerVerification>");
			stringBuilder.AppendLine("    <IncludeCompositeObjects>True</IncludeCompositeObjects>");
			stringBuilder.AppendLine("    <TargetDatabaseSet>True</TargetDatabaseSet>");
			stringBuilder.AppendLine("  </PropertyGroup>");
			stringBuilder.AppendLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
			stringBuilder.AppendLine("    <OutputPath>bin\\Release\\</OutputPath>");
			stringBuilder.AppendLine("    <BuildScriptName>$(MSBuildProjectName).sql</BuildScriptName>");
			stringBuilder.AppendLine("    <TreatWarningsAsErrors>False</TreatWarningsAsErrors>");
			stringBuilder.AppendLine("    <DebugType>pdbonly</DebugType>");
			stringBuilder.AppendLine("    <Optimize>true</Optimize>");
			stringBuilder.AppendLine("    <DefineDebug>false</DefineDebug>");
			stringBuilder.AppendLine("    <DefineTrace>true</DefineTrace>");
			stringBuilder.AppendLine("    <ErrorReport>prompt</ErrorReport>");
			stringBuilder.AppendLine("    <WarningLevel>4</WarningLevel>");
			stringBuilder.AppendLine($"    <TreatTSqlWarningsAsErrors>{Database.TreatTSqlWarningsAsErrors}</TreatTSqlWarningsAsErrors>");
			stringBuilder.AppendLine("  </PropertyGroup>");
			stringBuilder.AppendLine("  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
			stringBuilder.AppendLine("    <OutputPath>bin\\Debug\\</OutputPath>");
			stringBuilder.AppendLine("    <BuildScriptName>$(MSBuildProjectName).sql</BuildScriptName>");
			stringBuilder.AppendLine("    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>");
			stringBuilder.AppendLine("    <DebugSymbols>true</DebugSymbols>");
			stringBuilder.AppendLine("    <DebugType>full</DebugType>");
			stringBuilder.AppendLine("    <Optimize>false</Optimize>");
			stringBuilder.AppendLine("    <DefineDebug>true</DefineDebug>");
			stringBuilder.AppendLine("    <DefineTrace>true</DefineTrace>");
			stringBuilder.AppendLine("    <ErrorReport>prompt</ErrorReport>");
			stringBuilder.AppendLine("    <WarningLevel>4</WarningLevel>");
			stringBuilder.AppendLine($"    <TreatTSqlWarningsAsErrors>{Database.TreatTSqlWarningsAsErrors}</TreatTSqlWarningsAsErrors>");
			stringBuilder.AppendLine("  </PropertyGroup>");
			stringBuilder.AppendLine("  <PropertyGroup>");
			stringBuilder.AppendLine("    <VisualStudioVersion Condition=\"'$(VisualStudioVersion)' == ''\">11.0</VisualStudioVersion>");
			stringBuilder.AppendLine("    <SSDTExists Condition=\"Exists('$(MSBuildExtensionsPath)\\Microsoft\\VisualStudio\\v$(VisualStudioVersion)\\SSDT\\Microsoft.Data.Tools.Schema.SqlTasks.targets')\">True</SSDTExists>");
			stringBuilder.AppendLine("    <VisualStudioVersion Condition=\"'$(SSDTExists)' == ''\">11.0</VisualStudioVersion>");
			stringBuilder.AppendLine("  </PropertyGroup>");
			stringBuilder.AppendLine("  <Import Condition=\"'$(SQLDBExtensionsRefPath)' != ''\" Project=\"$(SQLDBExtensionsRefPath)\\Microsoft.Data.Tools.Schema.SqlTasks.targets\" />");
			stringBuilder.AppendLine("  <Import Condition=\"'$(SQLDBExtensionsRefPath)' == ''\" Project=\"$(MSBuildExtensionsPath)\\Microsoft\\VisualStudio\\v$(VisualStudioVersion)\\SSDT\\Microsoft.Data.Tools.Schema.SqlTasks.targets\" />");
			stringBuilder.AppendLine("  <ItemGroup>");
			stringBuilder.AppendLine($"    <Build Include=\"{Database.Name}\\**\\*.sql\" />");
			stringBuilder.AppendLine($"    <Folder Include=\"{Database.Name}\" />");
			stringBuilder.AppendLine("  </ItemGroup>");
			stringBuilder.AppendLine("  <ItemGroup>");

			foreach (DatabaseDependency dependency in Database.DatabaseDependencies)
			{
				switch (dependency.Type)
				{
					case DatabaseDependencyType.Dacpac:
						stringBuilder.Append(GetDacpacDependency(dependency.Reference));
						break;
					case DatabaseDependencyType.Database:
						stringBuilder.Append(GetDatabaseDependency(dependency.Reference));
						break;
					case DatabaseDependencyType.System:
						stringBuilder.Append(GetSystemDependency(dependency.Reference));
						break;
				}
			}

			stringBuilder.AppendLine("  </ItemGroup>");
			stringBuilder.AppendLine("  <ItemGroup>");

			if (Database.SchemaCompare != null)
				stringBuilder.AppendLine($"    <None Include=\"{Database.Name}.scmp\" />");

			stringBuilder.AppendLine("  </ItemGroup>");
			stringBuilder.AppendLine("  <ItemGroup>");
			stringBuilder.AppendLine(GetFolderIncludes());
			stringBuilder.AppendLine("  </ItemGroup>");
			stringBuilder.AppendLine("</Project>");

			Directory.CreateDirectory(sqlSolutionOutputPath);

			string sqlprojFilePath = Path.Combine(sqlSolutionOutputPath, $"{Database.Name}.sqlproj");
			File.WriteAllText(sqlprojFilePath, stringBuilder.ToString());
		}

		private static string GetDacpacDependency(string dacpacDependencyReference)
		{
			string dacpacFileName = Path.GetFileName(dacpacDependencyReference);

			StringBuilder stringBuilder = new();
			stringBuilder.AppendLine($"    <ArtifactReference Include=\"{dacpacFileName}\">");
			stringBuilder.AppendLine($"      <HintPath>{dacpacFileName}</HintPath>");
			stringBuilder.AppendLine("      <SuppressMissingDependenciesErrors>False</SuppressMissingDependenciesErrors>");
			stringBuilder.AppendLine($"      <DatabaseVariableLiteralValue>{Path.GetFileNameWithoutExtension(dacpacFileName)}</DatabaseVariableLiteralValue>");
			stringBuilder.AppendLine("    </ArtifactReference>");

			return stringBuilder.ToString();
		}

		private string GetDatabaseDependency(string databaseDependencyReference)
		{
			Project project = PreviouslyCreatedProjects.Find(proj => proj.Name == databaseDependencyReference)!;

			if (project == null)
				throw new InvalidOperationException($"Failed to find database object {databaseDependencyReference}, which database {Database.Name} depends on!");

			StringBuilder stringBuilder = new();
			stringBuilder.AppendLine($"    <ProjectReference Include=\"{project.Name}.sqlproj\">");
			stringBuilder.AppendLine($"      <Name>{project.Name}</Name>");
			stringBuilder.AppendLine($"      <Project>{project.Guid}</Project>");
			stringBuilder.AppendLine("      <Private>True</Private>");
			stringBuilder.AppendLine("      <SuppressMissingDependenciesErrors>False</SuppressMissingDependenciesErrors>");
			stringBuilder.AppendLine($"      <DatabaseVariableLiteralValue>{databaseDependencyReference}</DatabaseVariableLiteralValue>");
			stringBuilder.AppendLine("    </ProjectReference>");

			return stringBuilder.ToString();
		}

		private string GetSystemDependency(string systemDependencyReference)
		{
			string dacpacRootPath;

			if (Database.SqlServerPlatform == SqlServerPlatform.Azure)
				dacpacRootPath = "$(DacPacRootPath)";
			else
				dacpacRootPath = Path.Combine(@"C:\Program Files (x86)\Microsoft Visual Studio\2019", Properties.Instance.OtherProperties["VisualStudioEdition"], "Common7", "IDE");

			string dacpacSubFolder = Database.SqlServerPlatform == SqlServerPlatform.Azure ? "AzureV12" : $"{(int)Database.SqlServerVersion}0";

			string dacpacFullPath = Path.Combine
			(
				Path.Combine(dacpacRootPath, "Extensions", "Microsoft", "SQLDB"),
				Path.Combine("Extensions", "SqlServer", dacpacSubFolder, "SqlSchemas")
			);

			string hintPath;

			if (systemDependencyReference is "master" or "msdb")
				hintPath = Path.Combine(dacpacFullPath, $"{systemDependencyReference}.dacpac");
			else
				throw new InvalidOperationException($"Invalid system dependency {systemDependencyReference} referenced by database {Database.Name}!");

			StringBuilder stringBuilder = new();
			stringBuilder.AppendLine($"    <ArtifactReference Include=\"{hintPath}\">");
			stringBuilder.AppendLine($"      <HintPath>{hintPath}</HintPath>");
			stringBuilder.AppendLine("      <SuppressMissingDependenciesErrors>False</SuppressMissingDependenciesErrors>");
			stringBuilder.AppendLine($"      <DatabaseVariableLiteralValue>{systemDependencyReference}</DatabaseVariableLiteralValue>");
			stringBuilder.AppendLine("    </ArtifactReference>");

			return stringBuilder.ToString();
		}

		private string GetFolderIncludes()
		{
			List<string> includeLines = new()
			{
				$"    <Folder Include=\"{Database.Name}\\Security\\\" />"
			};

			// Iterate over the schema structure and only add each type of folder include a maximum of one time per schema.
			foreach (KeyValuePair<string, Schema> schema in Database.Schemas)
			{
				includeLines.Add($"    <Folder Include=\"{Database.Name}\\{schema.Key}\\\" />");

				foreach (KeyValuePair<string, DatabaseObject> output in schema.Value.DatabaseObjects)
				{
					if (output.Value is Function)
					{
						includeLines.Add($"    <Folder Include=\"{Database.Name}\\{schema.Key}\\Functions\\\" />");
						break;
					}
				}

				foreach (KeyValuePair<string, DatabaseObject> output in schema.Value.DatabaseObjects)
				{
					if (output.Value is Procedure)
					{
						includeLines.Add($"    <Folder Include=\"{Database.Name}\\{schema.Key}\\Stored Procedures\\\" />");
						break;
					}
				}

				foreach (KeyValuePair<string, DatabaseObject> output in schema.Value.DatabaseObjects)
				{
					if (output.Value is Table)
					{
						includeLines.Add($"    <Folder Include=\"{Database.Name}\\{schema.Key}\\Tables\\\" />");
						break;
					}
				}

				foreach (KeyValuePair<string, DatabaseObject> output in schema.Value.DatabaseObjects)
				{
					if (output.Value is View)
					{
						includeLines.Add($"    <Folder Include=\"{Database.Name}\\{schema.Key}\\Views\\\" />");
						break;
					}
				}
			}

			foreach (string folderIncludePath in Database.FolderIncludePaths)
			{
				string absoluteFolderPath;

				if (folderIncludePath.StartsWith(@"Nuget\", StringComparison.Ordinal))
				{
					string[] pathParts = folderIncludePath.Split(@"\");

					absoluteFolderPath = SqlGenerator.IonNugets[pathParts[1]] + folderIncludePath.Replace(@$"{pathParts[0]}\{pathParts[1]}", string.Empty, StringComparison.Ordinal);
				}
				else
					absoluteFolderPath = Path.Combine(Properties.Instance.ProjectDirectory!, folderIncludePath);

				foreach (string rootDirectory in Directory.GetDirectories(absoluteFolderPath))
				{
					string relativePathLine = $"    <Folder Include=\"{Database.Name}\\{new DirectoryInfo(rootDirectory).Name}\\\" />";

					if (!includeLines.Contains(relativePathLine))
						includeLines.Add(relativePathLine);

					foreach (string subDirectory in Directory.GetDirectories(rootDirectory))
					{
						DirectoryInfo subDirectoryInfo = new(subDirectory);

						string subDirectoryRelativePathLine = $"    <Folder Include=\"{Database.Name}\\{subDirectoryInfo.Parent!.Name}\\{subDirectoryInfo.Name}\\\" />";

						if (!includeLines.Contains(subDirectoryRelativePathLine))
							includeLines.Add(subDirectoryRelativePathLine);
					}
				}
			}

			return string.Join(Environment.NewLine, includeLines);
		}
	}
}
