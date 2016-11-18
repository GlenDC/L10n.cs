// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.IO;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace Parsers
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
