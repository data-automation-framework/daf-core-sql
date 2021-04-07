// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Sql.Output
{
	public class DatabaseDependency
	{
		public DatabaseDependency(string reference, DatabaseDependencyType type)
		{
			Reference = reference;
			Type = type;
		}

		public string Reference { get; }

		public DatabaseDependencyType Type { get; }
	}
}
