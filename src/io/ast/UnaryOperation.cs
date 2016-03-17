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
			public sealed class UnaryOperation : INode
			{
				INode m_Expression;
				Operation m_Operation;

				public UnaryOperation(INode expression, char op)
				{
					m_Expression = expression;

					switch (op) {
					case '+':
						m_Operation = Operation.Positive;
						break;

					case '-':
						m_Operation = Operation.Negative;
						break;

					case '!':
						m_Operation = Operation.Negate;
						break;

					default:
						throw new ParseException(
							String.Format("{0} is not a valid <unary> operation", op));
					}
				}
				
				public Objects.L20nObject Eval()
				{
					var expression = m_Expression.Eval();

					switch (m_Operation) {
					case Operation.Positive:
						return new Objects.PositiveExpression(expression).Optimize();
						
					case Operation.Negative:
						return new Objects.NegativeExpression(expression).Optimize();
						
					case Operation.Negate:
						return new Objects.NegateExpression(expression).Optimize();
					}

					throw new EvaluateException(
						String.Format("{0} is not a valid <unary> operation", m_Operation));
				}
				
				public string Display()
				{
					var op = (m_Operation == Operation.Positive ? '+' :
					           (m_Operation == Operation.Negative ? '-' : '!'));

					return string.Format("{0}{1}", op, m_Expression.Display ());
				}

				enum Operation
				{
					Positive, // +
					Negative, // -
					Negate,   // !
				}
			}
		}
	}
}
