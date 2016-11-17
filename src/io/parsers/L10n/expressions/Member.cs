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
					/// A parser combinator to parse any expression that's either
					/// a call-, property-, or parenthesis expression.
					/// We call it a member expression because it's part (and therefore a member)
					/// of a unary or binary expression.
					/// </summary>
					public static class Member
					{
						public static AST.L10n.INode Parse(CharStream stream)
						{
							var startingPos = stream.Position;

							try
							{
								AST.L10n.INode expression;

								if (Attribute.PeekAndParse(stream, out expression))
							// an attribute expression is always seperated by '::' and optionally with square brackets
							// around the actual attribute identifier, that allows the use of an expression
								{
									return expression;
								} else if (Property.PeekAndParse(stream, out expression))
							// a property expression is always seperated by a dot (`.`) or embraced by square brackets
							// and has to have at least 1 property (e.g.: `x.y` or `x[y]`),
							// more than 1 is also acceptable (e.g.: `x.y.z` or `x[y][z]` or `x[y].z` or `x.y[z]`).
								{
									return expression;
								} else
								{
									var member = Parenthesis.Parse(stream);

									if (Call.PeekAndParse(stream, member, out expression))
										return expression;
	
									return member;
								}
							} catch (Exception e)
							{
								string msg = String.Format(
								"something went wrong parsing an <member_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
								throw new Exceptions.ParseException(msg, e);
							}
						}
					}
				}
			}
		}
	}
}
