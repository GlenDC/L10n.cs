// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L10n
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
						public static L10n.IO.AST.INode Parse(CharStream stream, L10n.IO.AST.INode member)
						{
							var startingPos = stream.Position;

							try
							{
								// skip opening tag
								stream.SkipCharacter('(');
								WhiteSpace.Parse(stream, true);

								var call = new L10n.IO.AST.CallExpression(member);

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
								throw new ParseException(msg, e);
							}
						}

						private static L10n.IO.AST.INode ParseExpression(CharStream stream)
						{
							WhiteSpace.Parse(stream, true);
							var expression = Expression.Parse(stream);
							WhiteSpace.Parse(stream, true);
							return expression;
						}

						public static bool PeekAndParse(
						CharStream stream, L10n.IO.AST.INode member,
						out L10n.IO.AST.INode expression)
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
}
