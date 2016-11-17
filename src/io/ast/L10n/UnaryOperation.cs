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
			namespace L10n
			{
				/// <summary>
				/// The AST representation for a unary expression,
				/// applied on a single literal or boolean object, depending on the operation.
				/// More Inforamtion: <see cref="L20nCore.IO.Parsers.Expressions.Unary"/> 
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
				
					public Objects.L20nObject Eval()
					{
						var expression = m_Expression.Eval();

						switch (m_Operation)
						{
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
