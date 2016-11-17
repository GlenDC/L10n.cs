// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{	
			namespace L10n
			{
				/// <summary>
				/// The parser combinator used to parse all the whitespace.
				/// The resulting output does not get stored.
				/// </summary>
				public static class WhiteSpace
				{
					public static int Parse(CharStream stream, bool optional)
					{
						int pos = stream.Position;
						int n = stream.SkipWhile(char.IsWhiteSpace);
						if (!optional && n == 0)
						{
							throw stream.CreateException(
							"at least one whitespace character is required",
							pos - stream.Position);
						}

						return n;
					}
				}
			}
		}
	}
}
