// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Exceptions;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{
			namespace L10n
			{
				/// <summary>
				/// The combinator parser used to parse a comment and ignore the resulting output.
				/// </summary>
				public static class Comment
				{
					public static void Parse(CharStream stream)
					{
						if (!stream.SkipIfPossible('/') || !(stream.SkipIfPossible('*')))
						{
							throw stream.CreateException(
							"a comment has to be opened with '/*'");
						}

						char c;
						string content = "";
						while (stream.ReadNext(out c))
						{
							if (c == '*' && stream.SkipIfPossible('/'))
							{
								return;
							}
							content += c;
						}

						throw new ParseException(
						"a comment entry was opened, but not closed",
						stream.CreateEOFException());
					}

					public static bool PeekAndParse(CharStream stream)
					{
						if (stream.PeekNext() != '/' || stream.PeekNext(1) != '*')
						{
							return false;
						}

						Comment.Parse(stream);
						return true;
					}
				}
			}
		}
	}
}
