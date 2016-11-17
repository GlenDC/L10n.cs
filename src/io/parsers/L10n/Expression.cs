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
				/// <summary>
				/// The combinator parser used to parse any of the possible expreesions.
				/// This bubbles down all the way to the most primitive expression if needed.
				/// </summary>
				/// <remarks>
				/// An expression is the same as a conditional expression,
				/// according to the L20n spec it can ONLY ever be a conditional expression
				/// so there is no need to make 2 seperate parsers for it
				/// </remarks>
				public static class Expression
				{
					public static AST.L10n.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
					
						try
						{
							var condition = Expressions.Logic.Parse(stream);

							// check if we have an IfElse case or simply a logical expression
							string s;
							if (stream.ReadReg(@"\s*\?\s*", out s))
							{
								var first = Expression.Parse(stream);
								WhiteSpace.Parse(stream, true);
								stream.SkipCharacter(':');
								WhiteSpace.Parse(stream, true);
								var second = Expression.Parse(stream);
								return new AST.L10n.Conditional(condition, first, second);
							} else
							{ // it's simply a logical expression
								return condition;
							}
						} catch (Exception e)
						{
							string msg = String.Format(
							"something went wrong parsing an <expression> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
							throw new Exceptions.ParseException(msg, e);
						}
					}
				}
			}
		}
	}
}
