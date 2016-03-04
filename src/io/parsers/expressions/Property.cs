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
using System.Collections.Generic;

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{
			namespace Expressions
			{
				public class Property
				{
					public static AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
						
						try {
							var property = new AST.PropertyExpression(
								IdentifierExpression.Parse(stream));

							while(stream.SkipIfPossible('.')) {
								property.Add(Identifier.Parse(stream));
							}
							
							return property;
						}
						catch(Exception e) {
							string msg = String.Format(
								"something went wrong parsing an <property_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
							throw new L20n.Exceptions.ParseException(msg, e);
						}
					}
					
					public static bool PeekAndParse(
						CharStream stream, out AST.INode expression)
					{
						if (stream.PeekReg(@"[$@_a-zA-Z]\w*\.")) {
							expression = Property.Parse(stream);
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
