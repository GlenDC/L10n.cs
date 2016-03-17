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
				return CommonEval(m_First.Optimize(), m_Second.Optimize())
					.UnwrapOr(this);
			}
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return Option<L20nObject>.MapAll<L20nObject>((parameters) => {
					return CommonEval(parameters[0], parameters[1]);
				}, m_First.Eval(ctx), m_Second.Eval(ctx));
			}

			private Option<L20nObject> CommonEval(L20nObject first, L20nObject second)
			{
				// Are they literals?
				var output = Option<Literal>.MapAll<L20nObject>((literals) => {
					var result = Operation(literals[0].Value, literals[1].Value);
					return new Option<L20nObject>(result);
				}, first.As<Literal>(), second.As<Literal>());
				
				if(output.IsSet) return output;
				
				// Are they booleans?
				output = Option<BooleanValue>.MapAll<L20nObject>((booleans) => {
					var result = Operation(booleans[0].Value, booleans[1].Value);
					return new Option<L20nObject>(result);
				}, first.As<BooleanValue>(), second.As<BooleanValue>());
				
				if(output.IsSet) return output;
				
				// OK.... they better be strings!
				return Option<StringOutput>.MapAll<L20nObject>((strings) => {
					var result = Operation(strings[0].Value, strings[1].Value);
					return new Option<L20nObject>(result);
				}, first.As<StringOutput>(), second.As<StringOutput>());
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
