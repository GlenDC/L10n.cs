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
				namespace Expressions
				{
					/// <summary>
					/// The parser combinator to parse any Primitive value,
					/// meaning that it will use the relavant parser combinator to parse either
					/// a literal, external value or identifier expression.
					/// </summary>
					public static class Primary
					{
						public static AST.L10n.INode Parse(CharStream stream)
						{
							var startingPos = stream.Position;
						
							try
							{
								AST.L10n.INode primary;

								if (Literal.PeekAndParse(stream, out primary))
									return primary;

								if (Value.PeekAndParse(stream, out primary))
									return primary;
							
								return IdentifierExpression.Parse(stream);
							} catch (Exception e)
							{
								string msg = String.Format(
								"something went wrong parsing a <primary> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
								throw new Exceptions.ParseException(msg, e);
							}
						}

						public static bool Peek(CharStream stream)
						{
							return Literal.Peek(stream)
								|| Value.Peek(stream)
								|| IdentifierExpression.Peek(stream);
						}
					}
				}
			}
		}
	}
}
