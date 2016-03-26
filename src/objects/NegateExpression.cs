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
		public sealed class NegateExpression : L20nObject
		{	
			private readonly L20nObject m_Expression;
			private BooleanValue m_Output;
			
			public NegateExpression(L20nObject expression)
			{
				m_Expression = expression;
				m_Output = new BooleanValue();
			}

			public override L20nObject Optimize()
			{
				var expression = m_Expression.Optimize() as BooleanValue;
				if (expression == null)
					return this;

				m_Output.Value = !expression.Value;
				return m_Output;
			}
			
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				var expression = m_Expression.Eval(ctx) as BooleanValue;
				if (expression == null) {
					Logger.Warning("negation of non-valid boolean evaluation isn't allowed");
					return expression;
				}

				m_Output.Value = !expression.Value;
				return m_Output;
			}
		}
	}
}
