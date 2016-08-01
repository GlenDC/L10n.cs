// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// <see cref="L20nCore.Objects.LogicalExpression"/> represents the binary
		/// expressions: 'And' and 'Or'. These expression wrap around 2 L20nObjects
		/// that get/should be evaluated to <see cref="L20nCore.Objects.BooleanValue"/> results,
		/// and return a new <see cref="L20nCore.Objects.BooleanValue"/> with the result of its expression. 
		/// </summary>
		public abstract class LogicalExpression : L20nObject
		{	
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.LogicalExpression"/> class.
			/// </summary>
			public LogicalExpression(L20nObject first, L20nObject second)
			{
				m_First = first;
				m_Second = second;
			}

			/// <summary>
			/// Returns a BooleanValue with the result of this expression evaluation,
			/// using the evaluated results of the 2 wrapped L20nObjects.
			/// Returns <c>null</c> in case something went wrong.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{	
				return Operation(ctx);
			}

			// This operation will be overriden by the actual expression classes
			// This way we can allow the expressions to evaluate the wrapped objects
			// in a lazy manner, rather than evaluating them prematurely.
			protected abstract L20nObject Operation(LocaleContext ctx);

			protected readonly L20nObject m_First;
			protected readonly L20nObject m_Second;
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.AndExpression"/> represents the
		/// binary operation: And, and is applied on the evaluated results of
		/// the 2 wrapped up <see cref="L20nCore.Objects.L20nObject"/> values.
		/// </summary>
		public sealed class AndExpression : LogicalExpression
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.AndExpression"/> class.
			/// </summary>
			public AndExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
			}

			/// <summary>
			/// Optimizes to a BooleanValue result in case the result can be optimized away early.
			/// Otherwise this instance gets returned instead.
			/// </summary>
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

			/// <summary>
			/// Evaluates to a BooleanValue containing the result of the
			/// And Expression applied on the 2 wrapped <see cref="L20nCore.Objects.L20nObject"/> values.
			/// Returns <c>null</c> in case something went wrong.
			/// </summary>
			protected override L20nObject Operation(LocaleContext ctx)
			{
				var first = m_First.Eval(ctx) as BooleanValue;
				if (first == null || !first.Value)
					return first;
				return m_Second.Eval(ctx) as BooleanValue;
			}
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.OrExpression"/> represents the
		/// binary operation: Or, and is applied on the evaluated results of
		/// the 2 wrapped up <see cref="L20nCore.Objects.L20nObject"/> values.
		/// </summary>
		public sealed class OrExpression : LogicalExpression
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.OrExpression"/> class.
			/// </summary>
			public OrExpression(L20nObject a, L20nObject b)
			: base(a, b)
			{
			}

			/// <summary>
			/// Optimizes to a BooleanValue result in case the result can be optimized away early.
			/// Otherwise this instance gets returned instead.
			/// </summary>
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

			/// <summary>
			/// Evaluates to a BooleanValue containing the result of the
			/// Or Expression applied on the 2 wrapped <see cref="L20nCore.Objects.L20nObject"/> values.
			/// Returns <c>null</c> in case something went wrong.
			/// </summary>
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
