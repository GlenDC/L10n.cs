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
		/// Represents a branch where based on an evaluated condition it will
		/// return the evaluation of one of two given expressions.
		/// </summary>
		public sealed class IfElseExpression : L20nObject
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.IfElseExpression"/> class.
			/// </summary>
			public IfElseExpression(
				L20nObject condition,
				L20nObject if_true, L20nObject if_false)
			{
				m_Condition = condition;
				m_IfTrue = if_true;
				m_IfFalse = if_false;
			}

			/// <summary>
			/// Returns the optimization of one of the two wrapped up expressions in case
			/// the wrapped up condition can be optimized to a constant <see cref="L20nCore.Objects.BooleanValue"/>.
			/// Returns this instance if that's not the case.
			/// </summary>
			public override L20nObject Optimize()
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
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				var condition = m_Condition.Eval(ctx) as BooleanValue;
				if (condition == null)
					return condition;

				return condition.Value ? m_IfTrue.Eval(ctx)
								 : m_IfFalse.Eval(ctx);
			}

			private readonly L20nObject m_Condition;
			private readonly L20nObject m_IfTrue;
			private readonly L20nObject m_IfFalse;
		}
	}
}
