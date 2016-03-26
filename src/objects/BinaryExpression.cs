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

using L20nCore.Utils;
using L20nCore.Internal;
using L20nCore.Exceptions;

namespace L20nCore
{
	namespace Objects
	{
		public abstract class BinaryExpression : L20nObject
		{	
			private readonly L20nObject m_First;
			private readonly L20nObject m_Second;
			
			public BinaryExpression(L20nObject first, L20nObject second)
			{
				m_First = first;
				m_Second = second;
			}

			public override L20nObject Optimize()
			{
				var result = CommonEval(m_First.Optimize(), m_Second.Optimize());
				return result == null ? this : result;
			}
			
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return CommonEval(m_First.Eval(ctx), m_Second.Eval(ctx));
			}

			private L20nObject CommonEval(L20nObject first, L20nObject second)
			{
				// Are they literals?
				var l1 = first as Literal;
				var l2 = second as Literal;

				if(l1 != null && l2 != null)
					return Operation(l1.Value, l2.Value);
				
				// Are they booleans?
				var b1 = first as BooleanValue;
				var b2 = second as BooleanValue;
				
				if(b1 != null && b2 != null)
					return Operation(b1.Value, b2.Value);
				
				// Are they strings!
				var s1 = first as StringOutput;
				var s2 = second as StringOutput;
				
				if(s1 != null && s2 != null)
					return Operation(s1.Value, s2.Value);

				return null;
			}
			
			protected abstract BooleanValue Operation(int a, int b);
			protected abstract BooleanValue Operation(bool a, bool b);
			protected abstract BooleanValue Operation(string a, string b);
		}
		
		public sealed class IsEqualExpression : BinaryExpression
		{
			public IsEqualExpression(L20nObject a, L20nObject b)
			: base(a, b) {}
			
			protected override BooleanValue Operation(int a, int b)
			{
				return new BooleanValue(a == b);
			}
			
			protected override BooleanValue Operation(bool a, bool b)
			{
				return new BooleanValue(a == b);
			}
			
			protected override BooleanValue Operation(string a, string b)
			{
				return new BooleanValue(a == b);
			}
		}
		
		public sealed class IsNotEqualExpression : BinaryExpression
		{
			public IsNotEqualExpression(L20nObject a, L20nObject b)
			: base(a, b) {}
			
			protected override BooleanValue Operation(int a, int b)
			{
				return new BooleanValue(a != b);
			}
			
			protected override BooleanValue Operation(bool a, bool b)
			{
				return new BooleanValue(a != b);
			}
			
			protected override BooleanValue Operation(string a, string b)
			{
				return new BooleanValue(a != b);
			}
		}
	}
}
