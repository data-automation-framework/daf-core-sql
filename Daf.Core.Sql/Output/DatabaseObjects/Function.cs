// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Sql.Output.DatabaseObjects
{
	public class Function : DatabaseObject
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
		public Function(IonStructure.Function function, string databaseName) : base(databaseName, function.Schema, function.Name)
		{
			Statement = function.Statement;
		}

		public override string Path { get { return System.IO.Path.Combine(SchemaName, "Functions", $"{Name}.sql"); } }

		public string Statement { get; }

		public override string ToString()
		{
			return Statement;
		}
	}
}
