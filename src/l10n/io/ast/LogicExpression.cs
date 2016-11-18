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
				/// The AST representation for a logical (binary) expression.
				/// More Information: <see cref="L20nCore.L10n.IO.Parsers.Expressions.Logic"/>
				/// </summary>
				public sealed class LogicExpression : INode
				{
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
				
					public L10n.Objects.L10nObject Eval()
					{
						var first = m_First.Eval();
						var second = m_Second.Eval();
					
						switch (m_Operation)
						{
							case Operation.And:
								return new L10n.Objects.AndExpression(first, second).Optimize();
						
							case Operation.Or:
								return new L10n.Objects.OrExpression(first, second).Optimize();
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

					private readonly INode m_First;
					private readonly INode m_Second;
					private readonly Operation m_Operation;
				}
			}
		}
	}
}
