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
				public class Parenthesis
				{
					public static AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
						
						try {
							if(stream.SkipIfPossible('(')) {
								var e = Expression.Parse(stream);
								stream.SkipCharacter(')');
								return e;
							}
							else { // than we /should/ have a primary expressions
								return Primary.Parse(stream);
							}
						}
						catch(Exception e) {
							string msg = String.Format(
								"something went wrong parsing an <parenthesis_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
							throw new Exceptions.ParseException(msg, e);
						}
					}

					public static bool PeekAndParse(CharStream stream, out AST.INode expression)
					{
						if (stream.PeekNext () == '(' || Primary.Peek(stream)) {
							expression = Parenthesis.Parse(stream);
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
