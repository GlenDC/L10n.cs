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
		/// <see cref="L20nCore.Objects.NegateExpression"/> represent the binary negation expression.
		/// It wraps around a <see cref="L20nCore.Objects.L20nObject"/>,
		/// evaluates that to a <see cref="L20nCore.Objects.BooleanValue"/>,
		/// and returns the negation of that value in the form
		/// of another <see cref="L20nCore.Objects.BooleanValue"/>.
		/// </summary>
		public sealed class NegateExpression : L20nObject
		{	
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.NegateExpression"/> class.
			/// </summary>
			public NegateExpression(L20nObject expression)
			{
				m_Expression = expression;
				m_Output = new BooleanValue();
			}

			/// <summary>
			/// Optimizes to a negated <see cref="L20nCore.Objects.BooleanValue"/> result,
			/// in case the wrapped <see cref="L20nCore.Objects.L20nObject"/> can be optimized to
			/// a <see cref="L20nCore.Objects.BooleanValue"/> itself.
			/// Returns <c>this</c> instance otherwise.
			/// </summary>
			public override L20nObject Optimize()
			{
				var expression = m_Expression.Optimize() as BooleanValue;
				if (expression == null)
					return this;

				m_Output.Value = !expression.Value;
				return m_Output;
			}

			/// <summary>
			/// Evaluates to a negated <see cref="L20nCore.Objects.BooleanValue"/> result,
			/// in case the wrapped <see cref="L20nCore.Objects.L20nObject"/> can be evaluated to
			/// a <see cref="L20nCore.Objects.BooleanValue"/> itself.
			/// Returns <c>null</c> otherwise.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				var expression = m_Expression.Eval(ctx) as BooleanValue;
				if (expression == null)
				{
					Logger.Warning("negation of non-valid boolean evaluation isn't allowed");
					return expression;
				}

				m_Output.Value = !expression.Value;
				return m_Output;
			}

			private readonly L20nObject m_Expression;
			private BooleanValue m_Output;
		}
	}
}
