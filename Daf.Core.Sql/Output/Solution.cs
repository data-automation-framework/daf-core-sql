// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Daf.Core.Sql.Output
{
	public static class Solution
	{
		private const string SqlprojGuid = "{00D1A9C2-B5F0-4AF3-8072-F6C62B433612}";

		public static void Generate(string solutionName, List<Project> projects, string outputPath)
		{
			if (projects == null)
				throw new ArgumentNullException(nameof(projects));

			StringBuilder stringBuilder = new();
			stringBuilder.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
			stringBuilder.AppendLine("# Visual Studio Version 16");
			stringBuilder.AppendLine("VisualStudioVersion = 16.0.31112.23");
			stringBuilder.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");

			foreach (Project project in projects)
			{
				stringBuilder.AppendLine($"Project(\"{SqlprojGuid}\") = \"{project.Name}\", \"{project.Name}.sqlproj\", \"{project.Guid}\"");
				stringBuilder.AppendLine("EndProject");
			}

			stringBuilder.AppendLine("Global");
			stringBuilder.AppendLine("	GlobalSection(SolutionConfigurationPlatforms) = preSolution");
			stringBuilder.AppendLine("		Debug|Any CPU = Debug|Any CPU");
			stringBuilder.AppendLine("		Release|Any CPU = Release|Any CPU");
			stringBuilder.AppendLine("	EndGlobalSection");
			stringBuilder.AppendLine("	GlobalSection(ProjectConfigurationPlatforms) = postSolution");

			foreach (Project project in projects)
			{
				stringBuilder.AppendLine($"		{project.Guid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
				stringBuilder.AppendLine($"		{project.Guid}.Debug|Any CPU.Build.0 = Debug|Any CPU");
				stringBuilder.AppendLine($"		{project.Guid}.Debug|Any CPU.Deploy.0 = Debug|Any CPU");
				stringBuilder.AppendLine($"		{project.Guid}.Release|Any CPU.ActiveCfg = Release|Any CPU");
				stringBuilder.AppendLine($"		{project.Guid}.Release|Any CPU.Build.0 = Release|Any CPU");
				stringBuilder.AppendLine($"		{project.Guid}.Release|Any CPU.Deploy.0 = Release|Any CPU");
			}

			stringBuilder.AppendLine("	EndGlobalSection");
			stringBuilder.AppendLine("	GlobalSection(SolutionProperties) = preSolution");
			stringBuilder.AppendLine("		HideSolutionNode = FALSE");
			stringBuilder.AppendLine("	EndGlobalSection");
			stringBuilder.AppendLine("	GlobalSection(ExtensibilityGlobals) = postSolution");
			stringBuilder.AppendLine($"		SolutionGuid = {Guid.NewGuid().ToString("B").ToUpper(CultureInfo.InvariantCulture)}");
			stringBuilder.AppendLine("	EndGlobalSection");
			stringBuilder.AppendLine("EndGlobal");

			File.WriteAllText(Path.Combine(outputPath, $"{solutionName}.sln"), stringBuilder.ToString());
		}
	}
}
