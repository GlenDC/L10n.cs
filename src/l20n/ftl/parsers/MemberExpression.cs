// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace Parsers
			{	
				/// <summary>
				/// The combinator parser used to parse a member-expression.
				/// </summary>
				public static class MemberExpression
				{
					public static L20n.FTL.AST.MemberExpression Parse(CharStream stream, L20n.FTL.AST.StringPrimitive identifier)
					{
						stream.SkipCharacter('[');
						L20n.FTL.AST.StringPrimitive keyword = Keyword.Parse(stream);
						stream.SkipCharacter(']');

						return new L20n.FTL.AST.MemberExpression(identifier, keyword);
					}

					public static bool Peek(CharStream stream)
					{
						return stream.PeekNext() == '[';
					}
				}
			}
		}
	}
}
