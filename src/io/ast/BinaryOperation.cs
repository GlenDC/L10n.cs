// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using L20nCore.Exceptions;

namespace L20nCore
{
	namespace IO
	{
		namespace AST
		{
			/// <summary>
			/// The AST representation for a binary expression.
			/// More Information: <see cref="L20nCore.IO.Parsers.Expressions.Binary"/>
			/// </summary>
			public sealed class BinaryOperation : INode
			{
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
					switch (m_Operation)
					{
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
					switch (m_Operation)
					{
						case Operation.LessThan:
							op = "<";
							break;
						case Operation.GreaterThan:
							op = ">";
							break;
						case Operation.LessThanOrEqual:
							op = "<=";
							break;
						case Operation.GreaterThanOrEqual:
							op = ">=";
							break;
						case Operation.Add:
							op = "+";
							break;
						case Operation.Subtract:
							op = "-";
							break;
						case Operation.Multiply:
							op = "*";
							break;
						case Operation.Divide:
							op = "/";
							break;
						case Operation.Modulo:
							op = "%";
							break;
						case Operation.IsEqual:
							op = "==";
							break;
						case Operation.IsNotEqual:
							op = "!=";
							break;
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

				private readonly Operation m_Operation;
				private readonly INode m_First;
				private readonly INode m_Second;
			}
		}
	}
}
