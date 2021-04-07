// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Sql.Output.DatabaseObjects
{
	public class View : DatabaseObject, IStatement
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
		public View(IonStructure.View view, string databaseName) : base(databaseName, view.Schema, view.Name)
		{
			Statement = view.Statement;
		}

		public override string Path { get { return System.IO.Path.Combine(SchemaName, "Views", $"{Name}.sql"); } }

		public string Statement { get; }

		public override string ToString()
		{
			return Statement;
		}
	}
}
