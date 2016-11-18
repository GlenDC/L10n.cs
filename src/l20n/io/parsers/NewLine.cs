// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Exceptions;
using L20nCore.Common.IO;

namespace L20nCore
{
	namespace L20n
	{
		namespace IO
		{
			namespace Parsers
			{	
				/// <summary>
				/// The parser combinator used to parse all the newlines.
				/// The resulting output does not get stored.
				/// </summary>
				public static class NewLine
				{
					public static int Parse(CharStream stream, bool optional)
					{
						int pos = stream.Position;
						int n = stream.SkipWhile((x) => x == '\r' || x == '\n');
						if (!optional && n == 0)
						{
							throw stream.CreateException(
								"at least one newline character is required",
								pos - stream.Position);
						}
						
						return n;
					}
				}
			}
		}
	}
}
