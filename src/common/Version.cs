// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace Common
	{
		/// <summary>
		/// Defines the possible Localization versions L20n supports
		/// </summary>
		public enum Version
		{
			L10n = 0x10,	// legacy (and original)
			L20n = 0x20,	// default
		}

		/// <summary>
		/// A helper class providing static methods related to <seealso cref="L20nCore.Common.Version"/>.
		/// </summary>
		static class Versions
		{
			/// <summary>
			/// If possible, returns a version based on a case insensitive string.
			/// </summary>
			/// <exception cref="Exception">Gets thrown when given string was not recognized.</exception>
			public static Version FromString(String str)
			{
				if (AreVersionStringsEqual("L10n", str))
				{
					Logger.Warning("L10n format is deprecated, please use the L20n format if possible.");
					return Version.L10n;
				}
				if (AreVersionStringsEqual("L20n", str))
				{
					return Version.L20n;
				}

				throw new ImportException(String.Format("{0} is not a recognized version string", str));
			}

			/// <summary>
			/// Extension method that allows a version Enum instance to be converted to a string.
			/// </summary>
			public static String ToString(this Version v)
			{
				switch (v)
				{
					case Version.L10n:
						return "L10n";
					default:
						return "L20n";
				}
			}

			private static bool AreVersionStringsEqual(String str1, String str2)
			{
				return String.Compare(str1, str2, StringComparison.OrdinalIgnoreCase) == 0;
			}
		}
	}
}

