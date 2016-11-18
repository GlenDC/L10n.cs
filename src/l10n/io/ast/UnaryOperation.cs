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
				/// The AST representation for a unary expression,
				/// applied on a single literal or boolean object, depending on the operation.
				/// More Inforamtion: <see cref="L20nCore.L10n.IO.Parsers.Expressions.Unary"/> 
				/// </summary>
				public sealed class UnaryOperation : INode
				{
					public UnaryOperation(INode expression, char op)
					{
						m_Expression = expression;

						switch (op)
						{
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
				
					public L10n.Objects.L10nObject Eval()
					{
						var expression = m_Expression.Eval();

						switch (m_Operation)
						{
							case Operation.Positive:
								return new L10n.Objects.PositiveExpression(expression).Optimize();
						
							case Operation.Negative:
								return new L10n.Objects.NegativeExpression(expression).Optimize();
						
							case Operation.Negate:
								return new L10n.Objects.NegateExpression(expression).Optimize();
						}

						throw new EvaluateException(
						String.Format("{0} is not a valid <unary> operation", m_Operation));
					}
				
					public string Display()
					{
						var op = (m_Operation == Operation.Positive ? '+' :
					           (m_Operation == Operation.Negative ? '-' : '!'));

						return string.Format("{0}{1}", op, m_Expression.Display());
					}

					enum Operation
					{
						Positive, // +
						Negative, // -
						Negate,   // !
					}

					private readonly INode m_Expression;
					private readonly Operation m_Operation;
				}
			}
		}
	}
}
