// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Text;
using Daf.Core.Sql.IonStructure;
using Daf.Core.Sql.Output.DatabaseObjects.Components;

namespace Daf.Core.Sql.Output.DatabaseObjects
{
	public class Table : DatabaseObject
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
		public Table(IonStructure.Table table, string databaseName) : base(databaseName, table.Schema, table.Name)
		{
			if (table == null)
				throw new ArgumentNullException(nameof(table));

			CompressionType = table.CompressionType;

			// Create columns and foreign keys.
			foreach (TableColumnBase column in table.TableColumns)
			{
				if (column is TableColumn tableColumn)
					Columns[tableColumn.Name] = new Column(tableColumn, this);
				else if (column is IonStructure.ForeignKey foreignKey)
				{
					Column foreignKeyColumn = new(foreignKey, this);
					Columns[foreignKeyColumn.Name] = foreignKeyColumn;

					ForeignKeys.Add(new Components.ForeignKey(foreignKey, foreignKeyColumn));
				}
			}

			// Don't create the PrimaryKey before the columns have been created.
			if (table.PrimaryKey != null)
				PrimaryKey = new Components.PrimaryKey(table.PrimaryKey, this);

			foreach (IonStructure.Index index in table.Indexes)
				Indexes.Add(new Components.Index(index, this));

			foreach (Trigger trigger in table.Triggers)
				Triggers.Add(trigger.Statement);
		}

		public TableCompressionTypeEnum CompressionType { get; }

		public Dictionary<string, Column> Columns { get; } = new();

		public ICollection<Components.ForeignKey> ForeignKeys { get; } = new List<Components.ForeignKey>();

		public ICollection<Components.Index> Indexes { get; } = new List<Components.Index>();

		public override string Path { get { return System.IO.Path.Combine(SchemaName, "Tables", $"{Name}.sql"); } }

		public Components.PrimaryKey? PrimaryKey { get; private set; }

		public ICollection<string> Triggers { get; } = new List<string>();

		public string TwoPartName { get { return $"[{SchemaName}].[{Name}]"; } }

		public override string ToString()
		{
			StringBuilder stringBuilder = new();
			stringBuilder.AppendLine($"CREATE TABLE {TwoPartName}");
			stringBuilder.AppendLine("(");

			foreach (Column column in Columns.Values)
				stringBuilder.AppendLine(column.ToString());

			if (PrimaryKey != null)
				stringBuilder.AppendLine(PrimaryKey.ToString());

			stringBuilder.AppendLine(")");
			stringBuilder.AppendLine("ON [PRIMARY]");

			if (CompressionType is TableCompressionTypeEnum.ColumnStore or TableCompressionTypeEnum.ColumnStoreArchive)
			{
				stringBuilder.AppendLine("GO").AppendLine();
				stringBuilder.AppendLine($"CREATE CLUSTERED COLUMNSTORE INDEX CCSIX_{Name} ON {TwoPartName}");
			}

			stringBuilder.AppendLine($"WITH (DATA_COMPRESSION = {CompressionType});");
			stringBuilder.AppendLine("GO").AppendLine();

			foreach (Components.ForeignKey foreignKey in ForeignKeys)
				stringBuilder.AppendLine(foreignKey.ToString());

			foreach (Components.Index index in Indexes)
				stringBuilder.AppendLine(index.ToString());

			foreach (string trigger in Triggers)
			{
				stringBuilder.AppendLine(trigger);
				stringBuilder.AppendLine("GO").AppendLine();
			}

			return stringBuilder.ToString();
		}
	}
}
