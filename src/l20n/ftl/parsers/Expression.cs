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
				/// The combinator parser used to parse an expression.
				/// An expression can be a quoted-pattern, number, identifier,
				/// variable, call-expression and member-expression.
				/// </summary>
				public static class Expresson
				{
					public static L20n.FTL.AST.INode Parse(CharStream stream)
					{
						L20n.FTL.AST.INode result;
						
						if (Identifier.PeekAndParse(stream, out result))
						{
							if (MemberExpression.Peek(stream))
								return MemberExpression.Parse(stream, result as L20n.FTL.AST.StringPrimitive);
							
							if (CallExpression.Peek(stream))
								return CallExpression.Parse(stream, result as L20n.FTL.AST.StringPrimitive);
							
							return result;
						}
						
						if (Variable.PeekAndParse(stream, out result))
							return result;

						if (QuotedPattern.PeekAndParse(stream, out result))
							return result;

						if (Number.PeekAndParse(stream, out result))
							return result;

						throw stream.CreateException(
							"no <expression> could be found, while one was expected");
					}
				}
			}
		}
	}
}
