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
				/// The combinator parser used to parse a comment.
				/// </summary>
				public static class Comment
				{
					public static L20n.FTL.AST.Comment Parse(CharStream stream)
					{
						stream.SkipCharacter('#');
						string value = stream.ReadWhile((x) => !NewLine.Predicate(x));
						return new L20n.FTL.AST.Comment(value);
					}
						
					public static bool PeekAndParse(CharStream stream, out L20n.FTL.AST.INode comment)
					{
						if (stream.PeekNext() != '#')
						{
							comment = null;
							return false;
						}
							
						comment = Comment.Parse(stream);
						return true;
					}
				}
			}
		}
	}
}
