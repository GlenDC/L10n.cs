// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;
using System.Text.RegularExpressions;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace Parsers
			{	
				/// <summary>
				/// The combinator parser used to parse any of the possible expreesions.
				/// This bubbles down all the way to the most primitive expression if needed.
				/// </summary>
				/// <remarks>
				/// An expression is the same as a conditional expression,
				/// according to the L10n spec it can ONLY ever be a conditional expression
				/// so there is no need to make 2 seperate parsers for it
				/// </remarks>
				public static class Expression
				{
					public static L10n.IO.AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
					
						try
						{
							var condition = Expressions.Logic.Parse(stream);

							// check if we have an IfElse case or simply a logical expression
							string s;
							if (stream.ReadReg(s_RegIfElsePeek, out s))
							{
								var first = Expression.Parse(stream);
								WhiteSpace.Parse(stream, true);
								stream.SkipCharacter(':');
								WhiteSpace.Parse(stream, true);
								var second = Expression.Parse(stream);
								return new L10n.IO.AST.Conditional(condition, first, second);
							} else
							{ // it's simply a logical expression
								return condition;
							}
						} catch (Exception e)
						{
							string msg = String.Format(
							"something went wrong parsing an <expression> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
							throw new ParseException(msg, e);
						}
					}

					private static readonly Regex s_RegIfElsePeek = new Regex(@"\s*\?\s*");
				}
			}
		}
	}
}
