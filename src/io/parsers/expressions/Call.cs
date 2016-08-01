// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{
			namespace Expressions
			{
				/// <summary>
				/// A parser combinator to parse call expressions, which retrieves
				/// a macro value and passes the values for the relevant parameters,
				/// in order to be able to evaluate that macro expression.
				/// </summary>
				public static class Call
				{
					public static AST.INode Parse(CharStream stream, AST.INode member)
					{
						var startingPos = stream.Position;

						try
						{
							// skip opening tag
							stream.SkipCharacter('(');
							WhiteSpace.Parse(stream, true);

							var call = new AST.CallExpression(member);

							// parse arguments if possible
							if (!stream.SkipIfPossible(')'))
							{
								call.AddParameter(ParseExpression(stream));

								// but we can also have moreß
								while (stream.SkipIfPossible(','))
								{
									call.AddParameter(ParseExpression(stream));
								}

								// skip closing tag
								stream.SkipCharacter(')');
							}

							return call;
						} catch (Exception e)
						{
							string msg = String.Format(
								"something went wrong parsing an <call_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
							throw new Exceptions.ParseException(msg, e);
						}
					}

					private static AST.INode ParseExpression(CharStream stream)
					{
						WhiteSpace.Parse(stream, true);
						var expression = Expression.Parse(stream);
						WhiteSpace.Parse(stream, true);
						return expression;
					}

					public static bool PeekAndParse(
						CharStream stream, AST.INode member,
						out AST.INode expression)
					{
						if (stream.PeekNext() == '(')
						{
							expression = Call.Parse(stream, member);
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
