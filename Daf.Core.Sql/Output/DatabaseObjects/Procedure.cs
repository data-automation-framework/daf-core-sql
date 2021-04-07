// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Sql.Output.DatabaseObjects
{
	public class Procedure : DatabaseObject
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
		public Procedure(IonStructure.Procedure procedure, string databaseName) : base(databaseName, procedure.Schema, procedure.Name)
		{
			Statement = procedure.Statement;
		}

		public override string Path { get { return System.IO.Path.Combine(SchemaName, "Stored Procedures", $"{Name}.sql"); } }

		public string Statement { get; }

		public override string ToString()
		{
			return Statement;
		}
	}
}
