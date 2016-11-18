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
				/// The combinator parser used to parse a comment and ignore the resulting output.
				/// </summary>
				public static class Comment
				{
					public static void Parse(CharStream stream)
					{
						stream.SkipCharacter('#');
						stream.ReadUntilEnd();
					}
						
					public static bool PeekAndParse(CharStream stream)
					{
						if (stream.PeekNext() != '#')
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
