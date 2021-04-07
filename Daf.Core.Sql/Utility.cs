// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Sql
{
	internal class Utility
	{
		/// <summary>
		/// Adds brackets to a string, but only if it doesn't already have them.
		/// </summary>
		/// <param name="input">The string that brackets should be added to</param>
		/// <returns>The same string, with brackets added</returns>
		internal static string AddBrackets(string input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			bool bracketStart = input.StartsWith("[", StringComparison.Ordinal);
			bool bracketEnd = input.EndsWith("]", StringComparison.Ordinal);

			if (bracketStart && bracketEnd)
				return input;
			else if (!bracketStart && !bracketEnd)
				return $"[{input}]";
			else
				throw new InvalidOperationException($"The input string ({input}) either started or ended with a bracket, but not both. It is invalid.");
		}

		internal static string FindStringBetweenStrings(string text, string start, string end)
		{
			int p1 = text.IndexOf(start, StringComparison.Ordinal) + start.Length;
			int p2 = text.IndexOf(end, p1, StringComparison.Ordinal);

			if (string.IsNullOrEmpty(end))
				return text.Substring(p1);
			else
				return text.Substring(p1, p2 - p1);
		}

		/// <summary>
		/// SQL Server doesn't allow object names longer than 128 characters.
		/// This method can be used to shorten object names in a deterministic manner.
		/// </summary>
		/// <param name="input">The string to shorten</param>
		/// <returns>The shortened string</returns>
		internal static string ShortenIfTooLong(string input)
		{
			if (input.Length > 128)
				return input.Substring(0, 116) + "_" + Math.Abs(GetDeterministicHashCode(input)) + "]";
			else
				return input;
		}

		// Grabbed from https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
		private static int GetDeterministicHashCode(string input)
		{
			unchecked
			{
				int hash1 = (5381 << 16) + 5381;
				int hash2 = hash1;

				for (int i = 0; i < input.Length; i += 2)
				{
					hash1 = ((hash1 << 5) + hash1) ^ input[i];

					if (i == input.Length - 1)
						break;

					hash2 = ((hash2 << 5) + hash2) ^ input[i + 1];
				}

				return hash1 + (hash2 * 1566083941);
			}
		}
	}
}
