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
		public abstract class LogicalExpression : L20nObject
		{	
			protected readonly L20nObject m_First;
			protected readonly L20nObject m_Second;
			
			public LogicalExpression(L20nObject first, L20nObject second)
			{
				m_First = first;
				m_Second = second;
			}
			
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{	
				return Operation(ctx);
			}
			
			protected abstract L20nObject Operation(LocaleContext ctx);
		}
		
		public sealed class AndExpression : LogicalExpression
		{
			public AndExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
			}
			
			public override L20nObject Optimize()
			{
				var first = m_First.Optimize() as BooleanValue;
				var second = m_Second.Optimize() as BooleanValue;

				// first need to be set, otherwise there is no way to optimize an '&&' expression
				if (first != null)
				{
					// if first is false, than we can simply return false, as it will always result in false
					if (!first.Value)
					{
						return first;
						// otherwise let's see if second is set, if so, we can simply cache the result
					} else if (second != null)
					{
						return second;           
					}
				}

				return this;
			}
			
			protected override L20nObject Operation(LocaleContext ctx)
			{
				var first = m_First.Eval(ctx) as BooleanValue;
				if (first == null || !first.Value)
					return first;
				return m_Second.Eval(ctx) as BooleanValue;
			}
		}
		
		public sealed class OrExpression : LogicalExpression
		{
			public OrExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
			}
			
			public override L20nObject Optimize()
			{
				var first = m_First.Optimize() as BooleanValue;
				var second = m_Second.Optimize() as BooleanValue;
				
				// first need to be set, otherwise there is no way to optimize an '||' expression
				if (first != null)
				{
					// if first is true, than we can simply return true, as it will always result in true
					if (first.Value)
					{
						return first;
						// otherwise let's see if second is set, if so, we can simply cache the result
					} else if (second != null)
					{
						return second;
					}
				}

				return this;
			}
			
			protected override L20nObject Operation(LocaleContext ctx)
			{
				var first = m_First.Eval(ctx) as BooleanValue;
				if (first == null || first.Value)
					return first;
				return m_Second.Eval(ctx) as BooleanValue;
			}
		}
	}
}
