/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 - 2017 Glen De Cauwsemaecker (contact@glendc.com)
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

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{
			namespace Expressions
			{
				public class Attribute
				{
					public static Types.AST.Expression Parse(
						CharStream stream, Types.AST.Expression member)
					{
						var startingPos = stream.Position;
						
						try {
							Types.AST.Expression expression;
							
							// could be a raw-identifier (2) OR any other expression (1)
							stream.SkipString("::");
							if(stream.SkipIfPossible('[')) {
								expression = Expression.Parse(stream);
								stream.SkipCharacter(']');
							}
							else {
								var id = RawIdentifier.Parse(stream);
								var identifier = new Types.Internal.Expressions.Identifier(id);
								expression = new Types.AST.Expressions.Primary(identifier);
							}

							return new Types.AST.Expressions.Attribute(member, expression);
						}
						catch(Exception e) {
							string msg = String.Format(
								"something went wrong parsing an <attribute_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
							throw new L20n.Exceptions.ParseException(msg, e);
						}
					}

					public static bool PeekAndParse(
						CharStream stream, Types.AST.Expression member,
						out Types.AST.Expression expression)
					{
						if (stream.PeekNext () == ':') {
							expression = Attribute.Parse(stream, member);
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
