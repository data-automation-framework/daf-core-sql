// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;

namespace Daf.Core.Sql.Output.DatabaseObjects
{
	public class Schema : DatabaseObject
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
		public Schema(IonStructure.Schema schema, string databaseName) : base(databaseName, schema.Name, schema.Name)
		{
			if (schema == null)
				throw new ArgumentNullException(nameof(schema));

			Owner = schema.Owner;
		}

		public Dictionary<string, DatabaseObject> DatabaseObjects { get; } = new();

		public string Owner { get; }

		public override string Path { get { return System.IO.Path.Combine("Security", $"{Name}.sql"); } }

		public override string ToString()
		{
			return $"CREATE SCHEMA [{Name}] AUTHORIZATION {Owner};";
		}
	}
}
