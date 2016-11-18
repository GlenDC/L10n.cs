// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

using L20nCore.L10n.Internal;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace Parsers
			{
				/// <summary>
				/// The combinator parser used to parse any of the possible entries.
				/// This can result in a comment (ignored), import statement, macro or entity.
				/// </summary>
				public static class Entry
				{
					public static void Parse(CharStream stream, LocaleContext.Builder builder)
					{
						var startingPos = stream.Position;
					
						try
						{
							if (Comment.PeekAndParse(stream))
								return;

							// normally we keep the requirements of a format for a parser internal,
							// but in this case we have the same start for both a <macro> and an <entity>
							// so we simply have to make an exception in this case for performance reasons
							if (stream.SkipIfPossible('<'))
							{
								var identifier = Identifier.Parse(stream, true);

								if (Macro.PeekAndParse(stream, identifier, builder))
									return;

								// now it NEEDS to be a entitiy, else our input is simply invalid
								// knowing that we are already on a path of no return
								// because of the fact that we started parsing '<' and an identifier.
								Entity.Parse(stream, identifier, builder);
							} else
							{
								// it has to be an import statement at this point
								ImportStatement.Parse(stream, builder);
							}
						} catch (Exception e)
						{
							string msg = String.Format(
							"something went wrong parsing an <entry> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
							throw new ParseException(msg, e);
						}
					}
				}
			}
		}
	}
}
