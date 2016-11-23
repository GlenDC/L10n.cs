// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Exceptions;
using L20nCore.Common.IO;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace Parsers
			{	
				/// <summary>
				/// The parser combinator used to parse all the whitespace.
				/// The resulting output does not get stored.
				/// </summary>
				public static class WhiteSpace
				{
					public static int Parse(CharStream stream)
					{
						int n = stream.SkipWhile(char.IsWhiteSpace);
						return n;
					}
				}
			}
		}
	}
}
