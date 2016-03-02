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

using L20n.Internal;

namespace L20n
{
	namespace Objects
	{
		public abstract class LogicalExpression : L20nObject
		{	
			private readonly L20nObject m_First;
			private readonly L20nObject m_Second;
			
			public LogicalExpression(L20nObject first, L20nObject second)
			{
				m_First = first;
				m_Second = second;
			}
			
			public override L20nObject Eval(Context ctx, params L20nObject[] argv)
			{
				var first = m_First.Eval(ctx).As<BooleanValue>();
				var second = m_Second.Eval(ctx).As<BooleanValue>();
				
				return Operation(first.Value, second.Value);
			}
			
			protected abstract BooleanValue Operation(bool a, bool b);
		}
		
		public sealed class AndExpression : LogicalExpression
		{
			public AndExpression(L20nObject a, L20nObject b)
			: base(a, b) {}
			
			protected override BooleanValue Operation(bool a, bool b)
			{
				return new BooleanValue(a && b);
			}
		}
		
		public sealed class OrExpression : LogicalExpression
		{
			public OrExpression(L20nObject a, L20nObject b)
			: base(a, b) {}
			
			protected override BooleanValue Operation(bool a, bool b)
			{
				return new BooleanValue(a || b);
			}
		}
	}
}
