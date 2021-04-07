// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Sql
{
	public enum DatabaseDependencyType
	{
		Database,
		System,
		Dacpac
	}

	public enum SqlServerPlatform
	{
		Azure,
		SqlServer
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "Doesn't apply here.")]
	public enum SqlServerVersion
	{
		SqlServer2016 = 13,
		SqlServer2017 = 14,
		SqlServer2019 = 15
	}
}
