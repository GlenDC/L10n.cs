// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace L20nCore
{
	namespace IO
	{
		/// <summary>
		/// The main L20n parser, that simply reads all entries of a given L20n Resource File,
		/// and adds them to the given <see cref="L20nCore.Internal.LocaleContext+Builder"/>. 
		/// </summary>
		public class LocalizableObjectsList
		{
			/// <summary>
			/// Reads the L20n resource at the given path, and adds all the entries found in that resource,
			/// to the given <c>builder</c>.
			/// Throws a <see cref="L20nCore.Exceptions.ParseException"/> in case something went wrong,
			/// during the parsing of the given resource.
			/// </summary>
			public static void ImportAndParse(string path, Internal.LocaleContext.L10nBuilder builder)
			{
				try
				{
					using (CharStream stream = CharStream.CreateFromFile(path))
					{
						while (stream.InputLeft())
						{
							// Skip WhiteSpace
							Parsers.L10n.WhiteSpace.Parse(stream, true);
							
							// Read Entry
							Parsers.L10n.Entry.Parse(stream, builder);
						}
					}
				} catch (Exception exception)
				{
					throw new Exceptions.ParseException(
						String.Format("couldn't parse <localizble_objects_list> from file: {0}", path),
						exception);
				}
			}
		}
	}
}
