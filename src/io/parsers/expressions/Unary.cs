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
using L20n.Exceptions;

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{
			namespace Expressions
			{
				public class Unary
				{
					public static L20n.Objects.L20nObject Parse(CharStream stream)
					{
						var startingPos = stream.Position;
						
						try {
							char op;
							if(stream.SkipAnyIfPossible(new char[3]{'+', '-', '!'}, out op)) {
								WhiteSpace.Parse(stream, true);
								var expression = Unary.Parse(stream);
								return Unary.CreateUnaryExpression(expression, op);
							}
							else {
								return Member.Parse(stream);
							}
						}
						catch(Exception e) {
							string msg = String.Format(
								"something went wrong parsing an <unary_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
							throw new L20n.Exceptions.ParseException(msg, e);
						}
					}

					private static L20n.Objects.L20nObject CreateUnaryExpression(
						L20n.Objects.L20nObject expression, char op)
					{
						switch (op) {
						case '+':
							return new L20n.Objects.PositiveExpression(expression);

						case '-':
							return new L20n.Objects.NegativeExpression(expression);

						case '!':
							return new L20n.Objects.NegateExpression(expression);
							
						default:
							throw new ParseException(
								String.Format("{0} is not a valid <unary> operation", op));
						}
					}
				}
			}
		}
	}
}
