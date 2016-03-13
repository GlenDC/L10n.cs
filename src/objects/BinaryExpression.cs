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

using L20n.Utils;
using L20n.Internal;
using L20n.Exceptions;

namespace L20n
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
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return Option<L20nObject>.Map<L20nObject>((parameters) => {
					var first = parameters[0];
					var second = parameters[1];

					// need to be of the same type
					var expectedType = first.GetType();
					if (expectedType != second.GetType()) {
						Logger.WarningFormat(
							"BinaryExpression's 2nd argument is expected to be {0}, got {1}",
						    expectedType, second.GetType());
						return L20nObject.None;
					}
					
					// for now we accept literals, booleans and literals
					
					Literal literal;
					if (first.As(out literal)) {
						var result = Operation(literal.Value, second.As<Literal>().Value);
						return new Option<L20nObject>(result);
					}
					
					BooleanValue boolValue;
					if (first.As(out boolValue)) {
						var result = Operation(boolValue.Value, second.As<BooleanValue>().Value);
						return new Option<L20nObject>(result);
					}
					
					return new Option<L20nObject>(Operation(
						first.As<StringOutput>().Value,
						second.As<StringOutput>().Value));
				}, m_First.Eval(ctx), m_Second.Eval(ctx));
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
