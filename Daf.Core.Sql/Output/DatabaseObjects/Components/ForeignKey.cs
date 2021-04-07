// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Text;

namespace Daf.Core.Sql.Output.DatabaseObjects.Components
{
	public class ForeignKey
	{
		public ForeignKey(IonStructure.ForeignKey foreignKey, Column column)
		{
			if (foreignKey == null)
				throw new ArgumentNullException(nameof(foreignKey));

			if (column == null)
				throw new ArgumentNullException(nameof(column));

			Table = column.Table;
			ColumnName = column.BracketedName;
			Name = $"[FK_{Table.Name}_{foreignKey.TableName}_{foreignKey.Name}]";
			ReferencedTable = column.ReferencedTable!; // It's probably safe to assume ReferencedTable isn't null in this case.
			ReferencedColumnName = foreignKey.ColumnName;
		}

		public string ColumnName { get; }

		/// <summary>
		/// The name of the FK.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The referenced foreign column name.
		/// </summary>
		public string ReferencedColumnName { get; }

		/// <summary>
		/// The referenced foreign table.
		/// </summary>
		public Table ReferencedTable { get; }

		public Table Table { get; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new();
			stringBuilder.AppendLine($"ALTER TABLE {Table.TwoPartName} ADD CONSTRAINT {Utility.ShortenIfTooLong(Name)} FOREIGN KEY ({ColumnName})");
			stringBuilder.AppendLine($"REFERENCES {ReferencedTable.TwoPartName} ({ReferencedColumnName});");
			stringBuilder.AppendLine("GO").AppendLine();
			stringBuilder.AppendLine($"ALTER TABLE {Table.TwoPartName} NOCHECK CONSTRAINT {Utility.ShortenIfTooLong(Name)};");
			stringBuilder.AppendLine("GO").AppendLine();

			return stringBuilder.ToString();
		}
	}
}
