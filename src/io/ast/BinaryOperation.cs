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
		namespace AST
		{
			public sealed class BinaryOperation : INode
			{
				readonly Operation m_Operation;
				readonly INode m_First;
				readonly INode m_Second;
				
				public BinaryOperation(INode first, INode second, Operation op)
				{
					m_First = first;
					m_Second = second;
					m_Operation = op;
				}
				
				public Objects.L20nObject Eval()
				{
					var first = m_First.Eval();
					var second = m_Second.Eval();
					
					return CreateOperation(first, second).Optimize();
				}

				private Objects.L20nObject CreateOperation(Objects.L20nObject first, Objects.L20nObject second)
				{
					switch (m_Operation) {
					case Operation.LessThan:
						return new Objects.LessThanExpression(first, second);
						
					case Operation.GreaterThan:
						return new Objects.GreaterThanExpression(first, second);
						
					case Operation.LessThanOrEqual:
						return new Objects.LessThanOrEqualExpression(first, second);
						
					case Operation.GreaterThanOrEqual:
						return new Objects.GreaterThanOrEqualExpression(first, second);
						
					case Operation.Add:
						return new Objects.AddExpression(first, second);
						
					case Operation.Subtract:
						return new Objects.SubstractExpression(first, second);
						
					case Operation.Multiply:
						return new Objects.MultiplyExpression(first, second);
						
					case Operation.Divide:
						return new Objects.DivideExpression(first, second);
						
					case Operation.Modulo:
						return new Objects.ModuloExpression(first, second);
						
					case Operation.IsEqual:
						return new Objects.IsEqualExpression(first, second);
						
					case Operation.IsNotEqual:
						return new Objects.IsNotEqualExpression(first, second);
					}

					throw new EvaluateException(
						String.Format("{0} is not a valid <binary> operation", m_Operation));
				}
				
				public string Display()
				{
					string op = null;
					switch (m_Operation) {
					case Operation.LessThan: op = "<"; break;
					case Operation.GreaterThan: op = ">"; break;
					case Operation.LessThanOrEqual: op = "<="; break;
					case Operation.GreaterThanOrEqual: op = ">="; break;
					case Operation.Add: op = "+"; break;
					case Operation.Subtract: op = "-"; break;
					case Operation.Multiply: op = "*"; break;
					case Operation.Divide: op = "/"; break;
					case Operation.Modulo: op = "%"; break;
					case Operation.IsEqual: op = "=="; break;
					case Operation.IsNotEqual: op = "!="; break;
					}

					return string.Format("({0}{1}{2})",
						m_First.Display(), op, m_Second.Display());
				}
				
				public enum Operation
				{
					IsEqual = 0,				// ==
					IsNotEqual = 1,				// !=
					LessThan = 2,				// <
					GreaterThan = 3,			// >
					LessThanOrEqual = 4,		// <=
					GreaterThanOrEqual = 5,		// >=
					Multiply = 6,				// *
					Divide = 7,					// /
					Modulo = 8,					// %
					Add = 9,					// +
					Subtract = 10,				// -
				}
			}
		}
	}
}
