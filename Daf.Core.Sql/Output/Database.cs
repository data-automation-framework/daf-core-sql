// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Linq;
using Daf.Core.Sql.IonStructure;
using Daf.Core.Sql.Output.DatabaseObjects;

namespace Daf.Core.Sql.Output
{
	public class Database
	{
		public Database(IonStructure.Database database)
		{
			if (database == null)
				throw new ArgumentNullException(nameof(database));

			Name = database.Name;
			CreateProject = database.CreateProject;
			DatabaseDependencies = GetDatabaseDependencies(database.Dependencies);
			FolderIncludePaths = database.DatabaseIncludes.Select(item => item.Path).ToList();
			SqlServerPlatform = (SqlServerPlatform)database.TargetSqlServerPlatform;
			SqlServerVersion = (SqlServerVersion)database.TargetSqlServerVersion;
			TreatTSqlWarningsAsErrors = database.TreatTSqlWarningsAsErrors;

			if (database.SchemaCompare != null)
				SchemaCompare = new SchemaCompare(database.SchemaCompare);
		}

		public bool CreateProject { get; }

		public ICollection<DatabaseDependency> DatabaseDependencies { get; }

		public ICollection<string> FolderIncludePaths { get; }

		public string Name { get; }

		public SqlServerPlatform SqlServerPlatform { get; }

		public SqlServerVersion SqlServerVersion { get; }

		public SchemaCompare? SchemaCompare { get; }

		public Dictionary<string, DatabaseObjects.Schema> Schemas { get; } = new();

		public bool TreatTSqlWarningsAsErrors { get; }

		public void GenerateObjects(IonStructure.Database database)
		{
			if (database == null)
				throw new ArgumentNullException(nameof(database));

			if (!SqlGenerator.Databases.ContainsKey(database.Name))
				throw new InvalidOperationException($"Database {database.Name} doesn't exist in {nameof(SqlGenerator.Databases)}! Don't call GenerateObjects() before it does.");

			GenerateSchemas(database.Schemas);
			GenerateTables(database.Tables);
			GenerateViews(database.Views);
			GenerateFunctions(database.Functions);
			GenerateProcedures(database.Procedures);
		}

		private static List<DatabaseDependency> GetDatabaseDependencies(List<DatabaseDependencyBase> ionDependencies)
		{
			List<DatabaseDependency> dependencies = new();

			foreach (DatabaseDependencyBase dependency in ionDependencies)
			{
				switch (dependency)
				{
					case DacpacDependency:
						dependencies.Add(new DatabaseDependency(dependency.Name, DatabaseDependencyType.Dacpac));
						break;
					case IonStructure.DatabaseDependency:
						dependencies.Add(new DatabaseDependency(dependency.Name, DatabaseDependencyType.Database));
						break;
					case SystemDependency:
						dependencies.Add(new DatabaseDependency(dependency.Name, DatabaseDependencyType.System));
						break;
				}
			}

			return dependencies;
		}

		private DatabaseObjects.Schema GetSchema(DatabaseObject databaseObject)
		{
			if (!Schemas.ContainsKey(databaseObject.SchemaName))
				throw new InvalidOperationException($"{databaseObject.DatabaseName}.{databaseObject.SchemaName}.{databaseObject.Name} references non-existing schema {databaseObject.SchemaName}!");

			return Schemas[databaseObject.SchemaName];
		}

		private void GenerateFunctions(List<IonStructure.Function> functions)
		{
			foreach (IonStructure.Function ionFunction in functions)
			{
				DatabaseObjects.Function function = new(ionFunction, Name);
				GetSchema(function).DatabaseObjects.Add(function.Name, function);
			}
		}

		private void GenerateProcedures(List<IonStructure.Procedure> procedures)
		{
			foreach (IonStructure.Procedure ionProcedure in procedures)
			{
				DatabaseObjects.Procedure procedure = new(ionProcedure, Name);
				GetSchema(procedure).DatabaseObjects.Add(procedure.Name, procedure);
			}
		}

		private void GenerateSchemas(List<IonStructure.Schema> schemas)
		{
			foreach (IonStructure.Schema ionSchema in schemas)
			{
				DatabaseObjects.Schema schema = new(ionSchema, Name);
				Schemas.Add(schema.Name, schema);
			}
		}

		private void GenerateTables(List<IonStructure.Table> tables)
		{
			foreach (IonStructure.Table ionTable in tables)
			{
				DatabaseObjects.Table table = new(ionTable, Name);
				GetSchema(table).DatabaseObjects.Add(table.Name, table);
			}
		}

		private void GenerateViews(List<IonStructure.View> views)
		{
			foreach (IonStructure.View ionView in views)
			{
				DatabaseObjects.View view = new(ionView, Name);
				GetSchema(view).DatabaseObjects.Add(view.Name, view);
			}
		}
	}
}
