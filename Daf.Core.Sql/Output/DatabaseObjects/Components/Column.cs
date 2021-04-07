// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using Daf.Core.Sql.IonStructure;

namespace Daf.Core.Sql.Output.DatabaseObjects.Components
{
	public class Column
	{
		private const int DefaultPrecision = 18;
		private const int DefaultScale = 0;

		public Column(TableColumn column, Table table)
		{
			if (column == null)
				throw new ArgumentNullException(nameof(column));

			Name = column.Name;
			DataType = GetSqlDataType(column.DataType, column.Length, column.Precision, column.Scale);
			Nullable = column.IsNullable;
			Length = column.Length;
			Precision = column.Precision;
			Scale = column.Scale;

			Table = table;
		}

		public Column(IonStructure.ForeignKey foreignKeyColumn, Table table)
		{
			if (foreignKeyColumn == null)
				throw new ArgumentNullException(nameof(foreignKeyColumn));

			if (table == null)
				throw new ArgumentNullException(nameof(table));

			ReferencedColumn = SqlGenerator.GetReferencedColumn(table.DatabaseName, foreignKeyColumn);
			ReferencedTable = ReferencedColumn.Table;

			Name = foreignKeyColumn.Name;
			DataType = ReferencedColumn.DataType;
			Nullable = foreignKeyColumn.IsNullable;
			Length = ReferencedColumn.Length;
			Precision = ReferencedColumn.Precision;
			Scale = ReferencedColumn.Scale;

			Table = table;
		}

		public string BracketedName { get { return Utility.AddBrackets(Name); } }

		public string DataType { get; }

		public bool Nullable { get; }

		public int Length { get; }

		public int Precision { get; }

		public int Scale { get; }

		public string Name { get; }

		/// <summary>
		/// The referenced foreign column, if this is a foreign key column.
		/// </summary>
		private Column? ReferencedColumn { get; }

		/// <summary>
		/// The referenced foreign table, if this is a foreign key column.
		/// </summary>
		public Table? ReferencedTable { get; }

		public Table Table { get; }

		public override string ToString()
		{
			string dataType = DataType.Replace("(-1)", "(MAX)", StringComparison.Ordinal);
			string nullable = Nullable ? "NULL" : "NOT NULL";

			return $"\t{BracketedName} {dataType} {nullable},";
		}

		private static string GetSqlDataType(DataTypeEnum dataType, int length, int precision, int scale)
		{
			if (precision == -1)
				precision = DefaultPrecision;

			if (scale == -1)
				scale = DefaultScale;

			switch (dataType)
			{
				case DataTypeEnum.AnsiString:
					return $"varchar({length})";
				case DataTypeEnum.String:
					return $"nvarchar({length})";
				case DataTypeEnum.AnsiStringFixedLength:
					return $"char({length})";
				case DataTypeEnum.StringFixedLength:
					return $"nchar({length})";
				case DataTypeEnum.Binary:
					return $"binary({length})";
				case DataTypeEnum.Boolean:
					return "bit";
				case DataTypeEnum.Byte:
					return "tinyint";
				case DataTypeEnum.Currency:
					return "money";
				case DataTypeEnum.Date:
					return "date";
				case DataTypeEnum.DateTime:
					return "datetime";
				case DataTypeEnum.DateTime2:
					return $"datetime2({scale})";
				case DataTypeEnum.DateTimeOffset:
					return $"datetimeoffset({scale})";
				case DataTypeEnum.Decimal:
					return $"decimal({precision}, {scale})";
				case DataTypeEnum.Double:
					return "float";
				case DataTypeEnum.Int16:
				case DataTypeEnum.UInt16:
					return "smallint";
				case DataTypeEnum.Int32:
				case DataTypeEnum.UInt32:
					return "int";
				case DataTypeEnum.Int64:
				case DataTypeEnum.UInt64:
					return "bigint";
				case DataTypeEnum.Single:
					return "real";
				case DataTypeEnum.Time:
					return $"time({scale})";
				case DataTypeEnum.Guid:
					return "uniqueidentifier";
			}

			throw new InvalidOperationException($"Data type {dataType} is not supported by Daf.Core!");
		}
	}
}
