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
					public static L20n.Objects.L20nObject Parse(CharStream stream)
					{
						var startingPos = stream.Position;
						
						try {
							var identifiers = new List<L20n.Objects.Identifier>();
							var identifier = RawIdentifier.Parse(stream);
							identifiers.Add(identifier.As<L20n.Objects.Identifier>());

							while(stream.SkipIfPossible('.')) {
								identifier = RawIdentifier.Parse(stream);
								identifiers.Add(identifier.As<L20n.Objects.Identifier>());
							}
							
							return new L20n.Objects.PropertyExpression(identifiers);
						}
						catch(Exception e) {
							string msg = String.Format(
								"something went wrong parsing an <property_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
							throw new L20n.Exceptions.ParseException(msg, e);
						}
					}
					
					public static bool PeekAndParse(
						CharStream stream, out L20n.Objects.L20nObject expression)
					{
						if (stream.PeekReg(@"[_a-zA-Z]\w*\.")) {
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
