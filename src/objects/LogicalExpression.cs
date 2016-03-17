/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 - 2017 Glen De Cauwsemaecker (contact@glendc.com)
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
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{	
				return Operation(ctx);
			}
			
			protected abstract Option<L20nObject> Operation(LocaleContext ctx);
		}
		
		public sealed class AndExpression : LogicalExpression
		{
			public AndExpression(L20nObject a, L20nObject b)
			: base(a, b) {}
			
			public override L20nObject Optimize()
			{
				Option<BooleanValue> first, second;

				first = m_First.Optimize().As<BooleanValue>();
				second = m_Second.Optimize().As<BooleanValue>();

				// first need to be set, otherwise there is no way to optimize an '&&' expression
				if(first.IsSet) {
					var unwrapped = first.Unwrap();
					// if first is false, than we can simply return false, as it will always result in false
					if(!unwrapped.Value) {
						return unwrapped;
					// otherwise let's see if second is set, if so, we can simply cache the result
					} else if(second.IsSet) {
						var result = unwrapped.Value && second.Unwrap().Value;
						return new BooleanValue(result);
						                   
					}
				}

				return this;
			}
			
			protected override Option<L20nObject> Operation(LocaleContext ctx)
			{
				return m_First.Eval(ctx)
					.UnwrapAs<BooleanValue>().Map((a) => {
						if(!a.Value)
							return new Option<L20nObject>(a);
						
						return m_Second.Eval(ctx)
							.Map((b) => new Option<L20nObject>(b));
					});
			}
		}
		
		public sealed class OrExpression : LogicalExpression
		{
			public OrExpression(L20nObject a, L20nObject b)
			: base(a, b) {}
			
			public override L20nObject Optimize()
			{
				Option<BooleanValue> first, second;
				
				first = m_First.Optimize().As<BooleanValue>();
				second = m_Second.Optimize().As<BooleanValue>();
				
				// first need to be set, otherwise there is no way to optimize an '||' expression
				if(first.IsSet) {
					var unwrapped = first.Unwrap();
					// if first is true, than we can simply return true, as it will always result in true
					if(unwrapped.Value) {
						return unwrapped;
						// otherwise let's see if second is set, if so, we can simply cache the result
					} else if(second.IsSet) {
						var result = unwrapped.Value || second.Unwrap().Value;
						return new BooleanValue(result);
						
					}
				}

				return this;
			}
			
			protected override Option<L20nObject> Operation(LocaleContext ctx)
			{
				return m_First.Eval(ctx)
					.UnwrapAs<BooleanValue>().Map((a) => {
						if(a.Value)
							return new Option<L20nObject>(a);
						
						return m_Second.Eval(ctx)
							.Map((b) => new Option<L20nObject>(b));
					});
			}
		}
	}
}
