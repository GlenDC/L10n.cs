// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using L20nCore.Exceptions;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{
			namespace Expressions
			{
				/// <summary>
				/// The expression parser combinator used to parse unary expressions,
				/// meaning that it will be an expression applied on only one element.
				/// </summary>
				public static class Unary
				{
					public static AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
						
						try
						{
							char op;
							if (stream.SkipAnyIfPossible(new char[3]{'+', '-', '!'}, out op))
							{
								WhiteSpace.Parse(stream, true);
								var expression = Unary.Parse(stream);
								return new AST.UnaryOperation(expression, op);
							} else
							{
								return Member.Parse(stream);
							}
						} catch (Exception e)
						{
							string msg = String.Format(
								"something went wrong parsing an <unary_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
							throw new Exceptions.ParseException(msg, e);
						}
					}
				}
			}
		}
	}
}
