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
using System.IO;

namespace L20n
{
	namespace Types
	{
		namespace AST
		{
			namespace Expressions
			{
				public class Binary : Expression
				{
					private enum Operation
					{
						Equal, // ==
						NotEqual, // !=
						LessThan, // <
						GreaterThan, // >
						LessThanOrEqual, // <=
						GreaterThanOrEqual, // >=
						Plus, // +
						Minus, // -
						Multiply, // *
						Divide, // /
						Modulo, // %
					}
					
					private readonly Expression m_First;
					private readonly Expression m_Second;
					private readonly Operation m_Operation;
					
					public Binary(Expression first, Expression second, string op)
					{
						switch (op) {
						case "==":
							m_Operation = Operation.Equal;
							break;
						case "!=":
							m_Operation = Operation.NotEqual;
							break;
						case "<":
							m_Operation = Operation.LessThan;
							break;
						case ">":
							m_Operation = Operation.GreaterThan;
							break;
						case "<=":
							m_Operation = Operation.LessThanOrEqual;
							break;
						case ">=":
							m_Operation = Operation.GreaterThanOrEqual;
							break;
						case "+":
							m_Operation = Operation.Plus;
							break;
						case "-":
							m_Operation = Operation.Minus;
							break;
						case "*":
							m_Operation = Operation.Multiply;
							break;
						case "/":
							m_Operation = Operation.Divide;
							break;
						case "%":
							m_Operation = Operation.Modulo;
							break;
						default:
							var msg = String.Format(
								"expected ('=='|'!='|'<='|'>='|'<'|'>'|'+'|'-'|'*'|'/'|'%'), got {0}", op);
							throw new IOException(msg);
						}
						
						m_First = first;
						m_Second = second;
					}
					
					public override bool Evaluate(out Internal.Expression output)
					{
						throw new Exception("TODO");
					}
				}
			}
		}
	}
}

