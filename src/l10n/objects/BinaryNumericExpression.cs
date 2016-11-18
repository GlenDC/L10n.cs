// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Utils;
using L20nCore.Common.Exceptions;

using L20nCore.L10n.Internal;

namespace L20nCore
{
	namespace L10n
	{
		namespace Objects
		{
			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.BinaryNumericExpression"/> represents a binary expression applied
			/// on two literal values that are the result of their evaluated L10nObject-counterparts.
			/// </summary>
			public abstract class BinaryNumericExpression : L10nObject
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.BinaryNumericExpression"/> class.
				/// </summary>
				public BinaryNumericExpression(L10nObject first, L10nObject second)
				{
					m_First = first;
					m_Second = second;
				}

				/// <summary>
				/// Optimizes to the result of this binary expression in case both
				/// wrapped objects can be optimized to <see cref="L20nCore.L10n.Objects.Literal"/> values.
				/// Returns <c>this</c> instance otherwise.
				/// </summary>
				public override L10nObject Optimize()
				{
					var first = m_First.Optimize() as Literal;
					var second = m_Second.Optimize() as Literal;

					if (first != null && second != null)
						return Operation(first.Value, second.Value);

					return this;
				}

				/// <summary>
				/// Evaluates to the result of this binary expression in case both
				/// wrapped objects can be evaluated to <see cref="L20nCore.L10n.Objects.Literal"/> values.
				/// Returns <c>null</c> in case something went wrong.
				/// </summary>
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
				{
					var first = m_First.Eval(ctx) as Literal;
					var second = m_Second.Eval(ctx) as Literal;
					
					if (first != null && second != null)
						return Operation(first.Value, second.Value);

					return null;
				}

				protected abstract L10nObject Operation(int a, int b);

				private readonly L10nObject m_First;
				private readonly L10nObject m_Second;
			}

			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.SubstractExpression"/> represents the
			/// arithmetic 'substract' expression and is applied
			/// on two literal values that are the result of their evaluated L10nObject-counterparts.
			/// The result of this expression is a single <see cref="L20nCore.L10n.Objects.Literal"/> value.
			/// </summary>
			public sealed class SubstractExpression : BinaryNumericExpression
			{
				Literal m_Output;

				public SubstractExpression(L10nObject a, L10nObject b)
				: base(a, b)
				{
					m_Output = new Literal();
				}

				protected override L10nObject Operation(int a, int b)
				{
					m_Output.Value = a - b;
					return m_Output;
				}
			}

			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.AddExpression"/> represents the
			/// arithmetic 'add' expression and is applied
			/// on two literal values that are the result of their evaluated L10nObject-counterparts.
			/// The result of this expression is a single <see cref="L10nCore.Objects.Literal"/> value.
			/// </summary>
			public sealed class AddExpression : BinaryNumericExpression
			{
				Literal m_Output;

				public AddExpression(L10nObject a, L10nObject b)
				: base(a, b)
				{
					m_Output = new Literal();
				}

				protected override L10nObject Operation(int a, int b)
				{
					m_Output.Value = a + b;
					return m_Output;
				}
			}

			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.MultiplyExpression"/> represents the
			/// arithmetic 'multiply' expression and is applied
			/// on two literal values that are the result of their evaluated L10nObject-counterparts.
			/// The result of this expression is a single <see cref="L20nCore.L10n.Objects.Literal"/> value.
			/// </summary>
			public sealed class MultiplyExpression : BinaryNumericExpression
			{
				Literal m_Output;

				public MultiplyExpression(L10nObject a, L10nObject b)
				: base(a, b)
				{
					m_Output = new Literal();
				}

				protected override L10nObject Operation(int a, int b)
				{
					m_Output.Value = a * b;
					return m_Output;
				}
			}

			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.DivideExpression"/> represents the
			/// arithmetic 'division' expression and is applied
			/// on two literal values that are the result of their evaluated L10nObject-counterparts.
			/// The result of this expression is a single <see cref="L20nCore.L10n.Objects.Literal"/> value.
			/// </summary>
			public sealed class DivideExpression : BinaryNumericExpression
			{
				Literal m_Output;

				public DivideExpression(L10nObject a, L10nObject b)
				: base(a, b)
				{
					m_Output = new Literal();
				}

				protected override L10nObject Operation(int a, int b)
				{
					m_Output.Value = a / b;
					return m_Output;
				}
			}

			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.ModuloExpression"/> represents the
			/// arithmetic 'modulo' expression and is applied
			/// on two literal values that are the result of their evaluated L10nObject-counterparts.
			/// The result of this expression is a single <see cref="L20nCore.L10n.Objects.Literal"/> value.
			/// </summary>
			public sealed class ModuloExpression : BinaryNumericExpression
			{
				Literal m_Output;

				public ModuloExpression(L10nObject a, L10nObject b)
				: base(a, b)
				{
					m_Output = new Literal();
				}

				protected override L10nObject Operation(int a, int b)
				{
					m_Output.Value = a % b;
					return m_Output;
				}
			}

			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.LessThanExpression"/> represents the
			/// comparision 'less than' expression and is applied
			/// on two literal values that are the result of their evaluated L10nObject-counterparts.
			/// The result of this expression is a single <see cref="L20nCore.L10n.Objects.BooleanValue"/> value,
			/// indicating wether or not the first element is smaller than the second element.
			/// </summary>
			public sealed class LessThanExpression : BinaryNumericExpression
			{
				BooleanValue m_Output;

				public LessThanExpression(L10nObject a, L10nObject b)
				: base(a, b)
				{
					m_Output = new BooleanValue();
				}

				protected override L10nObject Operation(int a, int b)
				{
					m_Output.Value = a < b;
					return m_Output;
				}
			}

			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.LessThanOrEqualExpression"/> represents the
			/// comparision 'less than or equal to' expression and is applied
			/// on two literal values that are the result of their evaluated L10nObject-counterparts.
			/// The result of this expression is a single <see cref="L20nCore.L10n.Objects.BooleanValue"/> value,
			/// indicating wether or not the first element is smaller or equal than/to the second element.
			/// </summary>
			public sealed class LessThanOrEqualExpression : BinaryNumericExpression
			{
				BooleanValue m_Output;

				public LessThanOrEqualExpression(L10nObject a, L10nObject b)
				: base(a, b)
				{
					m_Output = new BooleanValue();
				}

				protected override L10nObject Operation(int a, int b)
				{
					m_Output.Value = a <= b;
					return m_Output;
				}
			}

			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.GreaterThanExpression"/> represents the
			/// comparision 'grear than' expression and is applied
			/// on two literal values that are the result of their evaluated L10nObject-counterparts.
			/// The result of this expression is a single <see cref="L20nCore.L10n.Objects.BooleanValue"/> value,
			/// indicating wether or not the first element is greater than the second element.
			/// </summary>
			public sealed class GreaterThanExpression : BinaryNumericExpression
			{
				BooleanValue m_Output;

				public GreaterThanExpression(L10nObject a, L10nObject b)
				: base(a, b)
				{
					m_Output = new BooleanValue();
				}

				protected override L10nObject Operation(int a, int b)
				{
					m_Output.Value = a > b;
					return m_Output;
				}
			}

			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.GreaterThanOrEqualExpression"/> represents the
			/// comparision 'grear than or equal to' expression and is applied
			/// on two literal values that are the result of their evaluated L10nObject-counterparts.
			/// The result of this expression is a single <see cref="L20nCore.L10n.Objects.BooleanValue"/> value,
			/// indicating wether or not the first element is greater or equal than/to the second element.
			/// </summary>
			public sealed class GreaterThanOrEqualExpression : BinaryNumericExpression
			{
				BooleanValue m_Output;

				public GreaterThanOrEqualExpression(L10nObject a, L10nObject b)
				: base(a, b)
				{
					m_Output = new BooleanValue();
				}

				protected override L10nObject Operation(int a, int b)
				{
					m_Output.Value = a >= b;
					return m_Output;
				}
			}
		}
	}
}
