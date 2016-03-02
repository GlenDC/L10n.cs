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
				public class Binary
				{
					public static L20n.Objects.L20nObject Parse(CharStream stream)
					{
						var startingPos = stream.Position;
						
						try {
							var first = Unary.Parse(stream);
							string op;
							if(stream.ReadReg(@"\s*(\=\=|\!\=|\<\=|\>\=|\<|\>|\+|\-|\*|\/|\%)", out op)) {
								WhiteSpace.Parse(stream, true);
								var second = Binary.Parse(stream);
								return Binary.CreateBinaryExpression(first, second, op.Trim());
							}
							else {
								return first;
							}
						}
						catch(Exception e) {
							string msg = String.Format(
								"something went wrong parsing an <binary_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
							throw new L20n.Exceptions.ParseException(msg, e);
						}
					}

					private static L20n.Objects.L20nObject CreateBinaryExpression(
						L20n.Objects.L20nObject first, L20n.Objects.L20nObject second,
						string op)
					{
						switch (op) {
						case "<":
							return new L20n.Objects.LessThanExpression(first, second);

						case ">":
							return new L20n.Objects.GreaterThanExpression(first, second);

						case "<=":
							return new L20n.Objects.LessThanOrEqualExpression(first, second);

						case ">=":
							return new L20n.Objects.GreaterThanOrEqualExpression(first, second);
							
						case "+":
							return new L20n.Objects.AddExpression(first, second);
							
						case "-":
							return new L20n.Objects.SubstractExpression(first, second);
							
						case "*":
							return new L20n.Objects.MultiplyExpression(first, second);
							
						case "/":
							return new L20n.Objects.DivideExpression(first, second);
							
						case "%":
							return new L20n.Objects.ModuloExpression(first, second);

						case "==":
							return new L20n.Objects.IsEqualExpression(first, second);

						case "!=":
							return new L20n.Objects.IsNotEqualExpression(first, second);

						default:
							throw new ParseException(
								String.Format("{0} is not a valid <binary> operation", op));
						}
					}
				}
			}
		}
	}
}
