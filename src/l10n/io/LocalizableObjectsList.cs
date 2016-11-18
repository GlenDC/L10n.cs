// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

using L20nCore.L10n.IO.Parsers;
using L20nCore.L10n.Internal;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			/// <summary>
			/// The main L10n parser, that simply reads all entries of a given L10n Resource File,
			/// and adds them to the given <see cref="L20nCore.L10n.Internal.LocaleContext+Builder"/>. 
			/// </summary>
			public class LocalizableObjectsList
			{
				/// <summary>
				/// Reads the L10n resource at the given path, and adds all the entries found in that resource,
				/// to the given <c>builder</c>.
				/// Throws a <see cref="L20nCore.Common.Exceptions.ParseException"/> in case something went wrong,
				/// during the parsing of the given resource.
				/// </summary>
				public static void ImportAndParse(string path, LocaleContext.Builder builder)
				{
					try
					{
						using (CharStream stream = CharStream.CreateFromFile(path))
						{
							while (stream.InputLeft())
							{
								// Skip WhiteSpace
								WhiteSpace.Parse(stream, true);
							
								// Read Entry
								Entry.Parse(stream, builder);
							}
						}
					} catch (Exception exception)
					{
						throw new ParseException(
						String.Format("couldn't parse <localizble_objects_list> from file: {0}", path),
						exception);
					}
				}
			}
		}
	}
}
