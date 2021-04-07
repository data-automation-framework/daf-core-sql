// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using Daf.Core.Sql.IonStructure;

namespace Daf.Core.Sql.Output.DatabaseObjects.Components
{
	public class PrimaryKey
	{
		public PrimaryKey(IonStructure.PrimaryKey primaryKey, Table table)
		{
			if (primaryKey == null)
				throw new ArgumentNullException(nameof(primaryKey));

			if (table == null)
				throw new ArgumentNullException(nameof(table));

			Name = primaryKey.Name;
			Clustered = primaryKey.Clustered;

			if (!Clustered || (Clustered && table.CompressionType == TableCompressionTypeEnum.None))
				CompressionType = primaryKey.CompressionType;

			foreach (PrimaryKeyColumn column in primaryKey.TablePrimaryKeyColumns)
				Columns.Add(table.Columns[column.ColumnName]);
		}

		public string BracketedName { get { return Utility.AddBrackets(Name); } }

		public bool Clustered { get; }

		public TableCompressionTypeEnum CompressionType { get; }

		public ICollection<Column> Columns { get; } = new List<Column>();

		private string CommaSeparatedColumns
		{
			get
			{
				List<string> columns = new();

				foreach (Column column in Columns)
					columns.Add(column.BracketedName);

				return string.Join(", ", columns);
			}
		}

		public string Name { get; }

		public override string ToString()
		{
			string clustered = Clustered ? "CLUSTERED" : "NONCLUSTERED";

			return $"\tCONSTRAINT {BracketedName} PRIMARY KEY {clustered} ({CommaSeparatedColumns}) WITH (DATA_COMPRESSION = {CompressionType})";
		}
	}
}
