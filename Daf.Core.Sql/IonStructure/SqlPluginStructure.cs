// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Core.Sdk;

#nullable disable
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1720 // Identifier contains type name
#pragma warning disable CA1724 // Type names should not match namespaces
#pragma warning disable CA2227 // Collection properties should be read only. We likely need to make changes in the Ion parser before we can remove this.
namespace Daf.Core.Sql.IonStructure
{
	/// <summary>
	/// Root node for SQL databases.
	/// </summary>
	[IsRootNode]
	public class Sql
	{
		/// <summary>
		/// Collection of SQL database projects.
		/// </summary>
		public List<SqlProject> SqlProjects { get; set; }
	}

	/// <summary>
	/// SQL database project.
	/// </summary>
	public class SqlProject
	{
		/// <summary>
		/// Collection of all database definitions in the project.
		/// </summary>
		public List<Database> Databases { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }
	}

	public abstract class DatabaseDependencyBase
	{
		[IsRequired]
		public string Name { get; set; }
	}

	public class DacpacDependency : DatabaseDependencyBase { }

	public class DatabaseDependency : DatabaseDependencyBase { }

	public class SystemDependency : DatabaseDependencyBase { }

	/// <summary>
	/// Element in collection of all database definitions in the project.
	/// </summary>
	public class Database
	{
		/// <summary>
		/// Collection of all the paths containing SQL objects to be included in the .sqlproj file.
		/// </summary>
		public List<Folder> DatabaseIncludes { get; set; }

		/// <summary>
		/// Collection of all database dependencies.
		/// </summary>
		public List<DatabaseDependencyBase> Dependencies { get; set; }

		/// <summary>
		/// The database's schema compare (.scmp) file.
		/// </summary>
		public SchemaCompare SchemaCompare { get; set; }

		/// <summary>
		/// Collection of all database schema definitions in the project.
		/// </summary>
		public List<Schema> Schemas { get; set; }

		/// <summary>
		/// Collection of all table definitions in the project.
		/// </summary>
		public List<Table> Tables { get; set; }

		/// <summary>
		/// Collection of all view definitions in the project.
		/// </summary>
		public List<View> Views { get; set; }

		/// <summary>
		/// Collection of all function definitions in the project.
		/// </summary>
		public List<Function> Functions { get; set; }

		/// <summary>
		/// Collection of all procedure definitions in the project.
		/// </summary>
		public List<Procedure> Procedures { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The target SQL Server platform.
		/// </summary>
		[DefaultValue(TargetSqlServerPlatformEnum.SqlServer)]
		public TargetSqlServerPlatformEnum TargetSqlServerPlatform { get; set; }

		/// <summary>
		/// The target SQL Server version, if applicable.
		/// </summary>
		[DefaultValue(TargetSqlServerVersionEnum.SqlServer2019)]
		public TargetSqlServerVersionEnum TargetSqlServerVersion { get; set; }

		/// <summary>
		/// Whether T-SQL warnings should be treated as errors when building the SQL project.
		/// </summary>
		[DefaultValue(false)]
		public bool TreatTSqlWarningsAsErrors { get; set; }

		/// <summary>
		/// Whether a project file (.sqlproj) should be generated for this database.
		/// </summary>
		[DefaultValue(false)]
		public bool CreateProject { get; set; }
	}

	/// <summary>
	/// Element in collection of all the paths containing SQL objects to be included in the .sqlproj file.
	/// </summary>
	public class Folder
	{
		/// <summary>
		/// The file path of the folder to include.
		/// </summary>
		public string Path { get; set; }
	}

	/// <summary>
	/// The business key columns (unique constraint) of the target table.
	/// </summary>
	public class BusinessKeyColumn
	{
		/// <summary>
		/// The (or one of the) business key column of the target table.
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// The database's schema compare (.scmp) file.
	/// </summary>
	public class SchemaCompare
	{
		/// <summary>
		/// The connection string to use.
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Exclude database roles.
		/// </summary>
		[DefaultValue(false)]
		public bool ExcludeDatabaseRoles { get; set; }

		/// <summary>
		/// Exclude users.
		/// </summary>
		[DefaultValue(false)]
		public bool ExcludeUsers { get; set; }

		/// <summary>
		/// Exclude tables.
		/// </summary>
		[DefaultValue(false)]
		public bool ExcludeTables { get; set; }

		/// <summary>
		/// Exclude views.
		/// </summary>
		[DefaultValue(false)]
		public bool ExcludeViews { get; set; }

		/// <summary>
		/// Exclude stored procedures.
		/// </summary>
		[DefaultValue(false)]
		public bool ExcludeProcedures { get; set; }

		/// <summary>
		/// Exclude table-valued functions.
		/// </summary>
		[DefaultValue(false)]
		public bool ExcludeTableValuedFunctions { get; set; }

		/// <summary>
		/// Exclude scalar functions.
		/// </summary>
		[DefaultValue(false)]
		public bool ExcludeScalarFunctions { get; set; }
	}

	/// <summary>
	/// Element in collection of all database schema definitions in the project.
	/// </summary>
	public class Schema
	{
		/// <summary>
		/// The schema owner.
		/// </summary>
		public string Owner { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }
	}

	public abstract class TableColumnBase
	{
		/// <summary>
		/// The name of the object.
		/// </summary>
		[IsRequired]
		public string Name { get; set; }
	}

	public class TableColumn : TableColumnBase
	{
		/// <summary>
		/// The column's data type.
		/// </summary>
		[IsRequired]
		public DataTypeEnum DataType { get; set; }

		/// <summary>
		/// The length of this column's data type. This only applies to data type definitions that accept a length parameter, such as String and Binary types. Use -1 for MAX length.
		/// </summary>
		[DefaultValue(0)]
		public int Length { get; set; }

		/// <summary>
		/// The precision of this column's data type. This only applies to data type definitions that accept a precision parameter, such as Decimal.
		/// </summary>
		[DefaultValue(-1)]
		public int Precision { get; set; }

		/// <summary>
		/// The scale of this column's data type. This only applies to data type definitions that accept a scale parameter, such as Decimal.
		/// </summary>
		[DefaultValue(-1)]
		public int Scale { get; set; }

		/// <summary>
		/// Whether the column is nullable.
		/// </summary>
		[DefaultValue(false)]
		public bool IsNullable { get; set; }
	}

	public class ForeignKey : TableColumnBase
	{
		/// <summary>
		/// Hint info for the column being referenced. This is used for references to columns defined outside Daf.Core.
		/// </summary>
		public ColumnReferenceHint ColumnReferenceHint { get; set; }

		/// <summary>
		/// The name of the schema that the referenced foreign column's table is in.
		/// </summary>
		[IsRequired]
		public string SchemaName { get; set; }

		/// <summary>
		/// The name of the referenced foreign column's table.
		/// </summary>
		[IsRequired]
		public string TableName { get; set; }

		/// <summary>
		/// The name of the referenced foreign column.
		/// </summary>
		[IsRequired]
		public string ColumnName { get; set; }

		/// <summary>
		/// Foreign key constraint type.
		/// </summary>
		public string ForeignKeyConstraintMode { get; set; }

		/// <summary>
		/// Whether the column is nullable.
		/// </summary>
		[DefaultValue(false)]
		public bool IsNullable { get; set; }
	}

	public class MultipleColumnForeignKey : TableColumnBase
	{
		/// <summary>
		/// Hint info for the column being referenced. This is used for references to columns defined outside Daf.Core.
		/// </summary>
		public ColumnReferenceHint ColumnReferenceHint { get; set; }

		/// <summary>
		/// A foreign column reference.
		/// </summary>
		[IsRequired]
		public string ForeignColumnName { get; set; }

		/// <summary>
		/// All multiple column table reference columns that refer to foreign columns must have the same MultipleColumnTableReferenceGroupName.
		/// </summary>
		[IsRequired]
		public string MultipleColumnTableReferenceGroupName { get; set; }

		/// <summary>
		/// The type of foreign key constraint.
		/// </summary>
		public string ForeignKeyConstraintMode { get; set; }

		/// <summary>
		/// Whether the column is nullable.
		/// </summary>
		[DefaultValue(false)]
		public bool IsNullable { get; set; }
	}

	public class ColumnReferenceHint
	{
		/// <summary>
		/// The name of the schema that the table containing the referenced column belongs to.
		/// </summary>
		[IsRequired]
		public string SchemaName { get; set; }

		/// <summary>
		/// The name of the table containing the referenced column.
		/// </summary>
		[IsRequired]
		public string TableName { get; set; }

		/// <summary>
		/// The column's data type.
		/// </summary>
		[IsRequired]
		public DataTypeEnum DataType { get; set; }

		/// <summary>
		/// The length of this column's data type. This only applies to data type definitions that accept a length parameter, such as String and Binary types. Use -1 for MAX length.
		/// </summary>
		[DefaultValue(0)]
		public int Length { get; set; }

		/// <summary>
		/// The precision of this column's data type. This only applies to data type definitions that accept a precision parameter, such as Decimal.
		/// </summary>
		[DefaultValue(-1)]
		public int Precision { get; set; }

		/// <summary>
		/// The scale of this column's data type. This only applies to data type definitions that accept a scale parameter, such as Decimal.
		/// </summary>
		[DefaultValue(-1)]
		public int Scale { get; set; }
	}

	/// <summary>
	/// Element in collection of all table definitions in the project.
	/// </summary>
	public class Table
	{
		/// <summary>
		/// Collection of table columns.
		/// </summary>
		public List<TableColumnBase> TableColumns { get; set; }

		/// <summary>
		/// The table's primary key.
		/// </summary>
		public PrimaryKey PrimaryKey { get; set; }

		/// <summary>
		/// Collection of table indexes.
		/// </summary>
		public List<Index> Indexes { get; set; }

		/// <summary>
		/// Collection of table triggers.
		/// </summary>
		public List<Trigger> Triggers { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The table's schema.
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		/// The type of compression used by the table.
		/// </summary>
		[DefaultValue(TableCompressionTypeEnum.None)]
		public TableCompressionTypeEnum CompressionType { get; set; }
	}

	/// <summary>
	/// Element in collection of table keys.
	/// </summary>
	public class PrimaryKey
	{
		/// <summary>
		/// Collection of key column references.
		/// </summary>
		public List<PrimaryKeyColumn> TablePrimaryKeyColumns { get; set; }

		/// <summary>
		/// Whether to cluster on the primary key.
		/// </summary>
		[DefaultValue(false)]
		public bool Clustered { get; set; }

		/// <summary>
		/// The type of compression the primary key index uses.
		/// </summary>
		[DefaultValue(TableCompressionTypeEnum.None)]
		public TableCompressionTypeEnum CompressionType { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// Element in collection of key column references.
	/// </summary>
	public class PrimaryKeyColumn
	{
		/// <summary>
		/// The name of the referenced column.
		/// </summary>
		public string ColumnName { get; set; }
	}

	public enum TableCompressionTypeEnum
	{
		None,
		Row,
		Page,
		ColumnStore,
		ColumnStoreArchive,
	}

	/// <summary>
	/// Element in collection of table indexes.
	/// </summary>
	public class Index
	{
		/// <summary>
		/// Collection of column references to build the index on.
		/// </summary>
		public List<IndexColumn> TableIndexColumns { get; set; }

		/// <summary>
		/// Collection of column references to include in the index.
		/// </summary>
		public List<IndexInclude> TableIndexIncludes { get; set; }

		/// <summary>
		/// Specifies whether the index is clustered.
		/// </summary>
		[DefaultValue(false)]
		public bool Clustered { get; set; }

		/// <summary>
		/// Specifies whether each index value is unique.
		/// </summary>
		[DefaultValue(false)]
		public bool Unique { get; set; }

		/// <summary>
		/// The type of compression to use.
		/// </summary>
		[DefaultValue(TableCompressionTypeEnum.None)]
		public TableCompressionTypeEnum CompressionType { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// Element in collection of column references to build the index on.
	/// </summary>
	public class IndexColumn
	{
		/// <summary>
		/// The name of the referenced column.
		/// </summary>
		public string ColumnName { get; set; }

		/// <summary>
		/// The sort order of the column reference.
		/// </summary>
		[DefaultValue(SortOrderEnum.ASC)]
		public SortOrderEnum SortOrder { get; set; }
	}

	public enum SortOrderEnum
	{
		ASC,
		DESC,
	}

	/// <summary>
	/// Element in collection of column references to include in the index.
	/// </summary>
	public class IndexInclude
	{
		/// <summary>
		/// The name of the referenced column.
		/// </summary>
		public string ColumnName { get; set; }

		/// <summary>
		/// The sort order of the column reference.
		/// </summary>
		[DefaultValue(SortOrderEnum.ASC)]
		public SortOrderEnum SortOrder { get; set; }
	}

	/// <summary>
	/// Element in collection of table triggers.
	/// </summary>
	public class Trigger
	{
		/// <summary>
		/// The trigger's SQL statement.
		/// </summary>
		public string Statement { get; set; }
	}

	/// <summary>
	/// Element in collection of all view definitions in the project.
	/// </summary>
	public class View : Object { }

	/// <summary>
	/// 
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>")]
	public class Object
	{
		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The name of the schema this object belongs to.
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		/// The object's SQL statement.
		/// </summary>
		public string Statement { get; set; }
	}

	/// <summary>
	/// Element in collection of all function definitions in the project.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>")]
	public class Function : Object { }

	/// <summary>
	/// Element in collection of all procedure definitions in the project.
	/// </summary>
	public class Procedure : Object { }

	public enum TargetSqlServerPlatformEnum
	{
		Azure,
		SqlServer,
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "Doesn't apply here.")]
	public enum TargetSqlServerVersionEnum
	{
		SqlServer2016 = 13,
		SqlServer2017 = 14,
		SqlServer2019 = 15,
	}

	public enum DataTypeEnum
	{
		AnsiString,
		AnsiStringFixedLength,
		Binary,
		Byte,
		Boolean,
		Currency,
		Date,
		DateTime,
		DateTime2,
		DateTimeOffset,
		Decimal,
		Double,
		Guid,
		Int16,
		Int32,
		Int64,
		Object,
		SByte,
		Single,
		String,
		StringFixedLength,
		Time,
		UInt16,
		UInt32,
		UInt64,
		VarNumeric,
		Xml,
	}
}
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1720 // Identifier contains type name
#pragma warning restore CA1724 // Type names should not match namespaces
#pragma warning restore CA2227 // Collection properties should be read only. We likely need to make changes in the Ion parser before we can remove this.
