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
