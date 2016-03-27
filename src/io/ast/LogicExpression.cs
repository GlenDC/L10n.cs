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
using L20nCore.Exceptions;

namespace L20nCore
{
	namespace IO
	{
		namespace AST
		{
			public sealed class LogicExpression : INode
			{
				INode m_First;
				INode m_Second;
				Operation m_Operation;
				
				public LogicExpression(INode first, INode second, string op)
				{
					m_First = first;
					m_Second = second;
					
					switch (op)
					{
						case "&&":
							m_Operation = Operation.And;
							break;
						
						case "||":
							m_Operation = Operation.Or;
							break;
						
						default:
							throw new ParseException(
							String.Format("{0} is not a valid <logic> operation", op));
					}
				}
				
				public Objects.L20nObject Eval()
				{
					var first = m_First.Eval();
					var second = m_Second.Eval();
					
					switch (m_Operation)
					{
						case Operation.And:
							return new Objects.AndExpression(first, second).Optimize();
						
						case Operation.Or:
							return new Objects.OrExpression(first, second).Optimize();
					}
					
					throw new EvaluateException(
						String.Format("{0} is not a valid <unary> operation", m_Operation));
				}
				
				public string Display()
				{
					var op = (m_Operation == Operation.And ? "&&" : "||");
					return string.Format("{0}{1}{2}",
						m_First.Display(), op, m_Second.Display());
				}
				
				enum Operation
				{
					Or, // ||
					And, // &&
				}
			}
		}
	}
}
