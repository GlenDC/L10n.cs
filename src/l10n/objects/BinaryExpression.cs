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
			/// <see cref="L20nCore.L10n.Objects.BinaryExpression"/> defines a binary expression,
			/// which gets applied on two wrapped L10nObjects that are either,
			/// a Boolean, Literal or String on the precondition that both objects are from the same time.
			/// </summary>
			public abstract class BinaryExpression : L10nObject
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.BinaryExpression"/> class.
				/// </summary>
				public BinaryExpression(L10nObject first, L10nObject second)
				{
					m_First = first;
					m_Second = second;
					m_Output = new BooleanValue();
				}

				/// <summary>
				/// Optimizes to the result of this operation in case both
				/// wrapped objects can be optimized to a reconigzed primitive value.
				/// Returns <c>this</c> instance otherwise.
				/// </summary>
				public override L10nObject Optimize()
				{
					var result = CommonEval(m_First.Optimize(), m_Second.Optimize());
					return result == null ? this : result;
				}

				/// <summary>
				/// Evaluates the binary expression on the primtive resuls of the wrapped objects.
				/// Returns <c>null</c> in case something went wrong.
				/// </summary>
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
				{
					return CommonEval(m_First.Eval(ctx), m_Second.Eval(ctx));
				}

				/// <summary>
				/// A common evaluation method that defines the actual logic to cast
				/// both parameters to the same and accepted type (bool, literal or string).
				/// Returns null in case no legal match was found.
				/// </summary>
				private L10nObject CommonEval(L10nObject first, L10nObject second)
				{
					// Are they literals?
					var l1 = first as Literal;
					var l2 = second as Literal;

					if (l1 != null && l2 != null)
					{
						m_Output.Value = Operation(l1.Value, l2.Value);
						return m_Output;
					}

					// Are they booleans?
					var b1 = first as BooleanValue;
					var b2 = second as BooleanValue;

					if (b1 != null && b2 != null)
					{
						m_Output.Value = Operation(b1.Value, b2.Value);
						return m_Output;
					}

					// Are they strings!
					var s1 = first as StringOutput;
					var s2 = second as StringOutput;

					if (s1 != null && s2 != null)
					{
						m_Output.Value = Operation(s1.Value, s2.Value);
						return m_Output;
					}

					return null;
				}

				// The operation to be defined for Literals.
				protected abstract bool Operation(int a, int b);
				// The operation to be defined for BooleanValues.
				protected abstract bool Operation(bool a, bool b);
				// The operation to be defined for StringOutputs.
				protected abstract bool Operation(string a, string b);

				private readonly L10nObject m_First;
				private readonly L10nObject m_Second;
				private readonly BooleanValue m_Output;
			}

			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.IsEqualExpression"/> represents the binary equality operation.
			/// It compares 2 objects of an accepted type and returns a <see cref="L20nCore.L10n.Objects.BooleanValue"/>
			/// indicating wether or not the two values are equal to each other.
			/// What equality means depends on the exact object type.
			/// </summary>
			public sealed class IsEqualExpression : BinaryExpression
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.IsEqualExpression"/> class.
				/// </summary>
				public IsEqualExpression(L10nObject a, L10nObject b)
				: base(a, b)
				{
				}

				/// <summary>
				/// Returns <c>true</c> if the given integer values are equal,
				/// <c>false</c> otherwise.
				/// </summary>
				protected override bool Operation(int a, int b)
				{
					return a == b;
				}

				/// <summary>
				/// Returns <c>true</c> if the given boolean values are equal,
				/// <c>false</c> otherwise.
				/// </summary>
				protected override bool Operation(bool a, bool b)
				{
					return a == b;
				}

				/// <summary>
				/// Returns <c>true</c> if the given string values are equal,
				/// <c>false</c> otherwise.
				/// </summary>
				protected override bool Operation(string a, string b)
				{
					return a == b;
				}
			}

			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.IsNotEqualExpression"/> represents the binary inequality operation.
			/// It compares 2 objects of an accepted type and returns a <see cref="L20nCore.L10n.Objects.BooleanValue"/>
			/// indicating wether or not the two values are not equal to each other.
			/// What inequality means depends on the exact object type.
			/// </summary>
			public sealed class IsNotEqualExpression : BinaryExpression
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.IsNotEqualExpression"/> class.
				/// </summary>
				public IsNotEqualExpression(L10nObject a, L10nObject b)
				: base(a, b)
				{
				}

				/// <summary>
				/// Returns <c>true</c> if the given integer values are not equal,
				/// <c>false</c> otherwise.
				/// </summary>
				protected override bool Operation(int a, int b)
				{
					return a != b;
				}

				/// <summary>
				/// Returns <c>true</c> if the given boolean values are not equal,
				/// <c>false</c> otherwise.
				/// </summary>
				protected override bool Operation(bool a, bool b)
				{
					return a != b;
				}

				/// <summary>
				/// Returns <c>true</c> if the given string values are not equal,
				/// <c>false</c> otherwise.
				/// </summary>
				protected override bool Operation(string a, string b)
				{
					return a != b;
				}
			}
		}
	}
}
