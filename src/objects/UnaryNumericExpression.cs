// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// <see cref="L20nCore.Objects.UnaryNumericExpression"/> represents a unary expression
		/// applied the literal value that's the result of the evaluation on the wrapped L20nObject.
		/// </summary>
		public abstract class UnaryNumericExpression : L20nObject
		{	
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.UnaryNumericExpression"/> class.
			/// </summary>
			public UnaryNumericExpression(L20nObject expression)
			{
				m_Expression = expression;
				m_Output = new Literal();
			}

			/// <summary>
			/// In case the wrapped L20nObject can be optimized to a Literal value,
			/// a new Literal value will be returned on which the unary operation is applied.
			/// Otherwise this instance will be returned, without any optimization.
			/// </summary>
			public override L20nObject Optimize()
			{
				var literal = m_Expression.Optimize() as Literal;
				if (literal != null)
				{
					m_Output.Value = Operation(literal.Value);
					return m_Output;
				}

				return this;
			}

			/// <summary>
			/// Returns a literal value, that consists of an integer value that's the result of
			/// this unary operation applied on the evaluation of the wrapped L20nObject.
			/// <c>null</c> gets returned in case something went wrong.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				var literal = m_Expression.Eval(ctx) as Literal;
				if (literal == null)
				{
					Logger.Warning("couldn't operate on non-valid literal evaluation");
					return literal;
				}

				m_Output.Value = Operation(literal.Value);
				return m_Output;
			}
			
			protected abstract int Operation(int a);

			private readonly L20nObject m_Expression;
			private Literal m_Output;
		}

		/// <summary>
		/// An expression that doesn't do much, but is still here.
		/// </summary>
		public sealed class PositiveExpression : UnaryNumericExpression
		{
			public PositiveExpression(L20nObject e) : base(e)
			{
			}
			
			protected override int Operation(int a)
			{
				return +a;
			}
		}

		/// <summary>
		/// Negates the literal value. Meaning that negative values become positive,
		/// and positive values will become negative.
		/// </summary>
		public sealed class NegativeExpression : UnaryNumericExpression
		{
			public NegativeExpression(L20nObject e) : base(e)
			{
			}
			
			protected override int Operation(int a)
			{
				return -a;
			}
		}
	}
}
