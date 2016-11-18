// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace AST
			{
				/// <summary>
				/// The AST representation for a binary expression.
				/// More Information: <see cref="L20nCore.L10n.IO.Parsers.Expressions.Binary"/>
				/// </summary>
				public sealed class BinaryOperation : INode
				{
					public BinaryOperation(INode first, INode second, Operation op)
					{
						m_First = first;
						m_Second = second;
						m_Operation = op;
					}
				
					public L10n.Objects.L10nObject Eval()
					{
						var first = m_First.Eval();
						var second = m_Second.Eval();
					
						return CreateOperation(first, second).Optimize();
					}

					private L10n.Objects.L10nObject CreateOperation(L10n.Objects.L10nObject first, L10n.Objects.L10nObject second)
					{
						switch (m_Operation)
						{
							case Operation.LessThan:
								return new L10n.Objects.LessThanExpression(first, second);
						
							case Operation.GreaterThan:
								return new L10n.Objects.GreaterThanExpression(first, second);
						
							case Operation.LessThanOrEqual:
								return new L10n.Objects.LessThanOrEqualExpression(first, second);
						
							case Operation.GreaterThanOrEqual:
								return new L10n.Objects.GreaterThanOrEqualExpression(first, second);
						
							case Operation.Add:
								return new L10n.Objects.AddExpression(first, second);
						
							case Operation.Subtract:
								return new L10n.Objects.SubstractExpression(first, second);
						
							case Operation.Multiply:
								return new L10n.Objects.MultiplyExpression(first, second);
						
							case Operation.Divide:
								return new L10n.Objects.DivideExpression(first, second);
						
							case Operation.Modulo:
								return new L10n.Objects.ModuloExpression(first, second);
						
							case Operation.IsEqual:
								return new L10n.Objects.IsEqualExpression(first, second);
						
							case Operation.IsNotEqual:
								return new L10n.Objects.IsNotEqualExpression(first, second);
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
}
