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

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
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
					public static AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
						
						try
						{
							AST.INode expression;

							// "for now" a property expression is always seperated by a dot (`.`)
							// and has to have at least 1 property (e.g.: `x.y`),
							// more than 1 is also acceptable (e.g.: `x.y.z`).
							if (Property.PeekAndParse(stream, out expression))
							{
								return expression;
							} else
							{
								var member = Parenthesis.Parse(stream);
								
								if (Call.PeekAndParse(stream, member, out expression))
									return expression;

								// Attributes have been removed from the L20nCore.cs
								// spec as they don't seem to add any value
								// over a regular HashValue
								
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
