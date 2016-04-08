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
		/// <summary>
		/// <see cref="L20nCore.Objects.BinaryNumericExpression"/> represents a binary expression applied
		/// on two literal values that are the result of their evaluated L20nObject-counterparts.
		/// </summary>
		public abstract class BinaryNumericExpression : L20nObject
		{	
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.BinaryNumericExpression"/> class.
			/// </summary>
			public BinaryNumericExpression(L20nObject first, L20nObject second)
			{
				m_First = first;
				m_Second = second;
			}

			/// <summary>
			/// Optimizes to the result of this binary expression in case both
			/// wrapped objects can be optimized to <see cref="L20nCore.Objects.Literal"/> values.
			/// Returns <c>this</c> instance otherwise.
			/// </summary>
			public override L20nObject Optimize()
			{
				var first = m_First.Optimize() as Literal;
				var second = m_Second.Optimize() as Literal;

				if (first != null && second != null)
					return Operation(first.Value, second.Value);

				return this;
			}

			/// <summary>
			/// Evaluates to the result of this binary expression in case both
			/// wrapped objects can be evaluated to <see cref="L20nCore.Objects.Literal"/> values.
			/// Returns <c>null</c> in case something went wrong.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				var first = m_First.Eval(ctx) as Literal;
				var second = m_Second.Eval(ctx) as Literal;
				
				if (first != null && second != null)
					return Operation(first.Value, second.Value);

				return null;
			}

			protected abstract L20nObject Operation(int a, int b);

			private readonly L20nObject m_First;
			private readonly L20nObject m_Second;
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.SubstractExpression"/> represents the
		/// arithmetic 'substract' expression and is applied
		/// on two literal values that are the result of their evaluated L20nObject-counterparts.
		/// The result of this expression is a single <see cref="L20nCore.Objects.Literal"/> value.
		/// </summary>
		public sealed class SubstractExpression : BinaryNumericExpression
		{
			Literal m_Output;

			public SubstractExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
				m_Output = new Literal();
			}

			protected override L20nObject Operation(int a, int b)
			{
				m_Output.Value = a - b;
				return m_Output;
			}
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.AddExpression"/> represents the
		/// arithmetic 'add' expression and is applied
		/// on two literal values that are the result of their evaluated L20nObject-counterparts.
		/// The result of this expression is a single <see cref="L20nCore.Objects.Literal"/> value.
		/// </summary>
		public sealed class AddExpression : BinaryNumericExpression
		{
			Literal m_Output;

			public AddExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
				m_Output = new Literal();
			}

			protected override L20nObject Operation(int a, int b)
			{
				m_Output.Value = a + b;
				return m_Output;
			}
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.MultiplyExpression"/> represents the
		/// arithmetic 'multiply' expression and is applied
		/// on two literal values that are the result of their evaluated L20nObject-counterparts.
		/// The result of this expression is a single <see cref="L20nCore.Objects.Literal"/> value.
		/// </summary>
		public sealed class MultiplyExpression : BinaryNumericExpression
		{
			Literal m_Output;

			public MultiplyExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
				m_Output = new Literal();
			}

			protected override L20nObject Operation(int a, int b)
			{
				m_Output.Value = a * b;
				return m_Output;
			}
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.DivideExpression"/> represents the
		/// arithmetic 'division' expression and is applied
		/// on two literal values that are the result of their evaluated L20nObject-counterparts.
		/// The result of this expression is a single <see cref="L20nCore.Objects.Literal"/> value.
		/// </summary>
		public sealed class DivideExpression : BinaryNumericExpression
		{
			Literal m_Output;

			public DivideExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
				m_Output = new Literal();
			}

			protected override L20nObject Operation(int a, int b)
			{
				m_Output.Value = a / b;
				return m_Output;
			}
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.ModuloExpression"/> represents the
		/// arithmetic 'modulo' expression and is applied
		/// on two literal values that are the result of their evaluated L20nObject-counterparts.
		/// The result of this expression is a single <see cref="L20nCore.Objects.Literal"/> value.
		/// </summary>
		public sealed class ModuloExpression : BinaryNumericExpression
		{
			Literal m_Output;

			public ModuloExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
				m_Output = new Literal();
			}

			protected override L20nObject Operation(int a, int b)
			{
				m_Output.Value = a % b;
				return m_Output;
			}
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.LessThanExpression"/> represents the
		/// comparision 'less than' expression and is applied
		/// on two literal values that are the result of their evaluated L20nObject-counterparts.
		/// The result of this expression is a single <see cref="L20nCore.Objects.BooleanValue"/> value,
		/// indicating wether or not the first element is smaller than the second element.
		/// </summary>
		public sealed class LessThanExpression : BinaryNumericExpression
		{
			BooleanValue m_Output;

			public LessThanExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
				m_Output = new BooleanValue();
			}

			protected override L20nObject Operation(int a, int b)
			{
				m_Output.Value = a < b;
				return m_Output;
			}
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.LessThanOrEqualExpression"/> represents the
		/// comparision 'less than or equal to' expression and is applied
		/// on two literal values that are the result of their evaluated L20nObject-counterparts.
		/// The result of this expression is a single <see cref="L20nCore.Objects.BooleanValue"/> value,
		/// indicating wether or not the first element is smaller or equal than/to the second element.
		/// </summary>
		public sealed class LessThanOrEqualExpression : BinaryNumericExpression
		{
			BooleanValue m_Output;

			public LessThanOrEqualExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
				m_Output = new BooleanValue();
			}

			protected override L20nObject Operation(int a, int b)
			{
				m_Output.Value = a <= b;
				return m_Output;
			}
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.GreaterThanExpression"/> represents the
		/// comparision 'grear than' expression and is applied
		/// on two literal values that are the result of their evaluated L20nObject-counterparts.
		/// The result of this expression is a single <see cref="L20nCore.Objects.BooleanValue"/> value,
		/// indicating wether or not the first element is greater than the second element.
		/// </summary>
		public sealed class GreaterThanExpression : BinaryNumericExpression
		{
			BooleanValue m_Output;

			public GreaterThanExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
				m_Output = new BooleanValue();
			}

			protected override L20nObject Operation(int a, int b)
			{
				m_Output.Value = a > b;
				return m_Output;
			}
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.GreaterThanOrEqualExpression"/> represents the
		/// comparision 'grear than or equal to' expression and is applied
		/// on two literal values that are the result of their evaluated L20nObject-counterparts.
		/// The result of this expression is a single <see cref="L20nCore.Objects.BooleanValue"/> value,
		/// indicating wether or not the first element is greater or equal than/to the second element.
		/// </summary>
		public sealed class GreaterThanOrEqualExpression : BinaryNumericExpression
		{
			BooleanValue m_Output;

			public GreaterThanOrEqualExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
				m_Output = new BooleanValue();
			}

			protected override L20nObject Operation(int a, int b)
			{
				m_Output.Value = a >= b;
				return m_Output;
			}
		}
	}
}
