// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Utils;

using L20nCore.L10n.Internal;

namespace L20nCore
{
	namespace L10n
	{
		namespace Objects
		{
			/// <summary>
			/// Represents a branch where based on an evaluated condition it will
			/// return the evaluation of one of two given expressions.
			/// </summary>
			public sealed class IfElseExpression : L10nObject
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.IfElseExpression"/> class.
				/// </summary>
				public IfElseExpression(
					L10nObject condition,
					L10nObject if_true, L10nObject if_false)
				{
					m_Condition = condition;
					m_IfTrue = if_true;
					m_IfFalse = if_false;
				}

				/// <summary>
				/// Returns the optimization of one of the two wrapped up expressions in case
				/// the wrapped up condition can be optimized to a constant <see cref="L20nCore.L10n.Objects.BooleanValue"/>.
				/// Returns this instance if that's not the case.
				/// </summary>
				public override L10nObject Optimize()
				{
					var condition = m_Condition.Optimize() as BooleanValue;
					if (condition == null)
						return this;

					return condition.Value ? m_IfTrue.Optimize()
					       		 : m_IfFalse.Optimize();
				}

				/// <summary>
				/// Returns the evaluation of one of the two wrapped up expressions.
				/// Returns <c>null</c> in case something went wrong.
				/// </summary>
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
				{
					var condition = m_Condition.Eval(ctx) as BooleanValue;
					if (condition == null)
						return condition;

					return condition.Value ? m_IfTrue.Eval(ctx)
								 : m_IfFalse.Eval(ctx);
				}

				private readonly L10nObject m_Condition;
				private readonly L10nObject m_IfTrue;
				private readonly L10nObject m_IfFalse;
			}
		}
	}
}
