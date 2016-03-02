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
			// An expression is the same as a conditional expression,
			// according to the L20n spec it can ONLY ever be a conditional expression
			// so there is no need to make 2 seperate parsers for it
			public class Expression
			{
				public static L20n.Objects.L20nObject Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try {
						var condition = Expressions.Logical.Parse(stream);

						// check if we have an IfElse case or simply a logical expression
						string s;
						if (stream.ReadReg(@"\s*\?\s*", out s)) {
							var first = Expression.Parse (stream);
							WhiteSpace.Parse (stream, true);
							stream.SkipCharacter (':');
							WhiteSpace.Parse (stream, true);
							var second = Expression.Parse (stream);
							return new L20n.Objects.IfElseExpression(
								condition, first, second);
						} else { // it's simply a logical expression
							return condition;
						}
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing an <expression> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new L20n.Exceptions.ParseException(msg, e);
					}
				}
			}
		}
	}
}
