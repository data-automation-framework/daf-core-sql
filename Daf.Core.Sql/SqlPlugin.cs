// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Globalization;
using Daf.Core.Sql.IonStructure;
using Daf.Core.Sdk;
using Daf.Core.Sdk.Ion.Reader;

namespace Daf.Core.Sql
{
	public class SqlPlugin : IPlugin
	{
		public string Description { get => "Generates SQL Server database projects (.sqlproj)."; }
		public string Name { get => "SQL Plugin"; }
		public string TimeStamp { get => ThisAssembly.GitCommitDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); }
		public string Version { get => ThisAssembly.AssemblyInformationalVersion; }

		public int Execute()
		{
			System.Diagnostics.Stopwatch sqlProjectTimer = System.Diagnostics.Stopwatch.StartNew();

			IonReader<IonStructure.Sql> sqlProjectReader = new(Properties.Instance.FilePath!, typeof(IonStructure.Sql).Assembly);

			string fileName = Properties.Instance.FilePath!;

			if (sqlProjectReader.RootNodeExistInFile())
			{
				IonStructure.Sql sqlRootNode = sqlProjectReader.Parse();

				foreach (SqlProject sqlProject in sqlRootNode.SqlProjects)
					SqlGenerator.GenerateEverything(sqlProject);

				sqlProjectTimer.Stop();
				string duration = TimeSpan.FromMilliseconds(sqlProjectTimer.ElapsedMilliseconds).ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
				Console.WriteLine($"Finished generating SQL project for {fileName} in {duration}");
			}
			else
			{
				sqlProjectTimer.Stop();
				Console.WriteLine($"No root node for {Name} found in {fileName}, no output generated.");
			}

			return 0;
		}
	}
}
