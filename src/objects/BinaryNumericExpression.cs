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
		public abstract class BinaryNumericExpression : L20nObject
		{	
			private readonly L20nObject m_First;
			private readonly L20nObject m_Second;
			
			public BinaryNumericExpression(L20nObject first, L20nObject second)
			{
				m_First = first;
				m_Second = second;
			}

			public override L20nObject Optimize()
			{
				var first = m_First.Optimize() as Literal;
				var second = m_Second.Optimize() as Literal;

				if(first != null && second != null)
					return Operation(first.Value, second.Value);

				return this;
			}
			
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				var first = m_First.Eval(ctx) as Literal;
				var second = m_Second.Eval(ctx) as Literal;
				
				if(first != null && second != null)
					return Operation(first.Value, second.Value);

				return null;
			}

			protected abstract L20nObject Operation(int a, int b);
		}
		
		public sealed class SubstractExpression : BinaryNumericExpression
		{
			public SubstractExpression(L20nObject a, L20nObject b)
			: base(a, b) {}

			protected override L20nObject Operation(int a, int b)
			{
				return new Literal(a - b);
			}
		}
		
		public sealed class AddExpression : BinaryNumericExpression
		{
			public AddExpression(L20nObject a, L20nObject b)
			: base(a, b) {}

			protected override L20nObject Operation(int a, int b)
			{
				return new Literal(a + b);
			}
		}
		
		public sealed class MultiplyExpression : BinaryNumericExpression
		{
			public MultiplyExpression(L20nObject a, L20nObject b)
			: base(a, b) {}

			protected override L20nObject Operation(int a, int b)
			{
				return new Literal(a * b);
			}
		}
		
		public sealed class DivideExpression : BinaryNumericExpression
		{
			public DivideExpression(L20nObject a, L20nObject b)
			: base(a, b) {}

			protected override L20nObject Operation(int a, int b)
			{
				return new Literal(a / b);
			}
		}
		
		public sealed class ModuloExpression : BinaryNumericExpression
		{
			public ModuloExpression(L20nObject a, L20nObject b)
			: base(a, b) {}

			protected override L20nObject Operation(int a, int b)
			{
				return new Literal(a % b);
			}
		}
		
		public sealed class LessThanExpression : BinaryNumericExpression
		{
			public LessThanExpression(L20nObject a, L20nObject b)
			: base(a, b) {}

			protected override L20nObject Operation(int a, int b)
			{
				return new BooleanValue(a < b);
			}
		}
		
		public sealed class LessThanOrEqualExpression : BinaryNumericExpression
		{
			public LessThanOrEqualExpression(L20nObject a, L20nObject b)
			: base(a, b) {}

			protected override L20nObject Operation(int a, int b)
			{
				return new BooleanValue(a <= b);
			}
		}
		
		public sealed class GreaterThanExpression : BinaryNumericExpression
		{
			public GreaterThanExpression(L20nObject a, L20nObject b)
			: base(a, b) {}

			protected override L20nObject Operation(int a, int b)
			{
				return new BooleanValue(a > b);
			}
		}
		
		public sealed class GreaterThanOrEqualExpression : BinaryNumericExpression
		{
			public GreaterThanOrEqualExpression(L20nObject a, L20nObject b)
			: base(a, b) {}

			protected override L20nObject Operation(int a, int b)
			{
				return new BooleanValue(a >= b);
			}
		}
	}
}
