// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common;
using L20nCore.Common.Utils;

using L20nCore.L10n.Internal;

namespace L20nCore
{
	namespace L10n
	{
		namespace Objects
		{
			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.NegateExpression"/> represent the binary negation expression.
			/// It wraps around a <see cref="L20nCore.L10n.Objects.L20nObject"/>,
			/// evaluates that to a <see cref="L20nCore.L10n.Objects.BooleanValue"/>,
			/// and returns the negation of that value in the form
			/// of another <see cref="L20nCore.L10n.Objects.BooleanValue"/>.
			/// </summary>
			public sealed class NegateExpression : L10nObject
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.NegateExpression"/> class.
				/// </summary>
				public NegateExpression(L10nObject expression)
				{
					m_Expression = expression;
					m_Output = new BooleanValue();
				}

				/// <summary>
				/// Optimizes to a negated <see cref="L20nCore.L10n.Objects.BooleanValue"/> result,
				/// in case the wrapped <see cref="L20nCore.L10n.Objects.L20nObject"/> can be optimized to
				/// a <see cref="L20nCore.L10n.Objects.BooleanValue"/> itself.
				/// Returns <c>this</c> instance otherwise.
				/// </summary>
				public override L10nObject Optimize()
				{
					var expression = m_Expression.Optimize() as BooleanValue;
					if (expression == null)
						return this;

					m_Output.Value = !expression.Value;
					return m_Output;
				}

				/// <summary>
				/// Evaluates to a negated <see cref="L20nCore.L10n.Objects.BooleanValue"/> result,
				/// in case the wrapped <see cref="L20nCore.L10n.Objects.L20nObject"/> can be evaluated to
				/// a <see cref="L20nCore.L10n.Objects.BooleanValue"/> itself.
				/// Returns <c>null</c> otherwise.
				/// </summary>
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
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

				private readonly L10nObject m_Expression;
				private BooleanValue m_Output;
			}
		}
	}
}
