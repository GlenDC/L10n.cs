/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 Glen De Cauwsemaecker (contact@glendc.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
				public class Call
				{
					public static AST.INode Parse(CharStream stream, AST.INode member)
					{
						var startingPos = stream.Position;
						
						try
						{
							// skip opening tag
							stream.SkipCharacter('(');

							var call = new AST.CallExpression(
								member, ParseExpression(stream));

							// but we can also have more
							while (stream.SkipIfPossible(','))
							{
								call.AddParameter(ParseExpression(stream));
							}

							// skip closing tag
							stream.SkipCharacter(')');

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
