// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Text;
using Daf.Core.Sql.IonStructure;

namespace Daf.Core.Sql.Output.DatabaseObjects.Components
{
	public class Index
	{
		public Index(IonStructure.Index index, Table table)
		{
			if (index == null)
				throw new ArgumentNullException(nameof(index));

			if (table == null)
				throw new ArgumentNullException(nameof(table));

			Name = index.Name;
			Columns = index.TableIndexColumns;
			Unique = index.Unique;
			Clustered = index.Clustered;
			CompressionType = index.CompressionType;

			TableName = table.TwoPartName;
		}

		public bool Clustered { get; }

		public TableCompressionTypeEnum CompressionType { get; }

		public string Name { get; }

		public string TableName { get; }

		public bool Unique { get; }

		private ICollection<IndexColumn> Columns { get; }

		private string CommaSeparatedColumns
		{
			get
			{
				List<string> columns = new();

				foreach (IndexColumn column in Columns)
					columns.Add($"{Utility.AddBrackets(column.ColumnName)} {column.SortOrder}");

				return string.Join(", ", columns);
			}
		}

		public override string ToString()
		{
			string unique = Unique ? "UNIQUE " : "";
			string clustered = Clustered ? "CLUSTERED" : "NONCLUSTERED";

			StringBuilder stringBuilder = new();
			stringBuilder.AppendLine($"CREATE {unique}{clustered} INDEX [{Name}]");
			stringBuilder.AppendLine($"ON {TableName} ({CommaSeparatedColumns})");
			stringBuilder.AppendLine($"WITH (DATA_COMPRESSION = {CompressionType})");
			stringBuilder.AppendLine("ON [PRIMARY];");
			stringBuilder.AppendLine("GO").AppendLine();

			return stringBuilder.ToString();
		}
	}
}
