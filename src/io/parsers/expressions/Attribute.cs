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
				/// <summary>
				/// An attribute expressions looks for an attribute,
				/// which means it points to some metadata within an Entity.
				/// </summary>
				public static class Attribute
				{
					public static AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
						
						try
						{
							AST.INode root;
							if(!Property.PeekAndParse(stream, out root)) {
								root = IdentifierExpression.Parse(stream);
							}
							stream.SkipString("::");

							// we either have an expression or a simple identifier
							AST.INode identifier;
							if (stream.SkipIfPossible('['))
							{
								identifier = Expression.Parse(stream);
								stream.SkipCharacter(']');
							} else
							{
								identifier = new AST.Identifier(Identifier.Parse(stream, false));
							}

							// We can also have optionally a property expression,
							// starting with a simple identifier or straight away with an expression
							AST.PropertyExpression propertyExpression = null;
							if (stream.SkipIfPossible('.'))
							{
								propertyExpression = Property.Parse(stream) as AST.PropertyExpression;
							}
							else if (stream.SkipIfPossible('['))
							{
								var expression = Expression.Parse(stream);
								propertyExpression = new AST.PropertyExpression(expression);
								stream.SkipCharacter(']');
								Property.Parse(stream, propertyExpression);
							}

							return new AST.AttributeExpression(root, identifier, propertyExpression);
						} catch (Exception e)
						{
							string msg = String.Format(
								"something went wrong parsing an <property_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
							throw new Exceptions.ParseException(msg, e);
						}
					}
					
					public static bool PeekAndParse(
						CharStream stream, out AST.INode expression)
					{
						if (stream.PeekReg(@"[$@]?\w+(\.\w+)*::"))
						{
							expression = Attribute.Parse(stream);
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
