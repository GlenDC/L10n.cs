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
					/// A parser combinator to parse any object that's in-between parenthesis.
					/// </summary>
					public static class Parenthesis
					{
						public static AST.L10n.INode Parse(CharStream stream)
						{
							var startingPos = stream.Position;
						
							try
							{
								if (stream.SkipIfPossible('('))
								{
									var e = Expression.Parse(stream);
									stream.SkipCharacter(')');
									return e;
								} else
								{ // than we /should/ have a primary expressions
									return Primary.Parse(stream);
								}
							} catch (Exception e)
							{
								string msg = String.Format(
								"something went wrong parsing an <parenthesis_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
								throw new Exceptions.ParseException(msg, e);
							}
						}

						public static bool PeekAndParse(CharStream stream, out AST.L10n.INode expression)
						{
							if (stream.PeekNext() == '(' || Primary.Peek(stream))
							{
								expression = Parenthesis.Parse(stream);
								return true;
							}

							expression = null;
							return false;
						}
					}
				}
			}
		}
	}
}
