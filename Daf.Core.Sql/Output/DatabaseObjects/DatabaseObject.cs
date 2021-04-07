// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sql.Output.DatabaseObjects
{
	public class DatabaseObject
	{
		protected DatabaseObject(string databaseName, string schemaName, string name)
		{
			DatabaseName = databaseName;
			SchemaName = schemaName;
			Name = name;
		}

		public string DatabaseName { get; }

		public string SchemaName { get; }

		public string Name { get; }

		public virtual string Path { get { throw new InvalidOperationException("Only the derived implementations of Path should be used!"); } }
	}
}
