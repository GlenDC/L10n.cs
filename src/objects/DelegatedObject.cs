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
		/// <see cref="L20nCore.Objects.DelegatedObject"/> represents a callback to be used
		/// at translation time, returning an L20nObject, which can be null.
		/// </summary>
		public sealed class DelegatedObject<T> : L20nObject
			where T: L20nObject
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.DelegatedObject`1"/> class.
			/// </summary>
			public DelegatedObject(Delegate callback)
			{
				m_Callback = callback;
			}

			/// <summary>
			/// Can't optimize, returning <c>this</c> instance instead.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// Returns the result of the callback,
			/// returns null in case that result is <c>null</c> or if the stored callback is <c>null</c>.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Callback == null)
					return null;

				return m_Callback();
			}

			public delegate T Delegate();

			private readonly Delegate m_Callback;
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.DelegatedValue"/> represents a callback to be used
		/// at translation time, returning an L20nObject, which can be null.
		/// </summary>
		public abstract class DelegatedValue<ValueType, L20nType> : L20nObject
			where L20nType: L20nObject
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.DelegatedValue"/> class.
			/// </summary>
			public DelegatedValue(Delegate callback, L20nType cache)
			{
				m_Callback = callback;
				m_Output = cache;
			}

			/// <summary>
			/// Can't optimize, returning <c>this</c> instance instead.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// Returns the result of the callback,
			/// returns null in case that result is <c>null</c> or if the stored callback is <c>null</c>.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Callback == null)
					return null;

				SetValue(m_Callback());
				return m_Output;
			}

			protected abstract void SetValue(ValueType value);

			public delegate ValueType Delegate();

			private readonly Delegate m_Callback;
			// using this variable to reuse the same L20nObject Instance,
			// to help stay responsible with the GC
			protected L20nType m_Output;
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.DelegatedLiteral"/> represents a callback to be used
		/// at translation time, returning an integer, which will be rerturned as a Literal.
		/// </summary>
		public sealed class DelegatedLiteral : DelegatedValue<int, Literal>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.DelegatedLiteral"/> class.
			/// </summary>
			public DelegatedLiteral(Delegate callback)
				: base(callback, new Literal())
			{
			}

			protected override void SetValue(int value)
			{
				m_Output.Value = value;
			}
		}
		
		/// <summary>
		/// <see cref="L20nCore.Objects.DelegatedString"/> represents a callback to be used
		/// at translation time, returning a string, which will be rerturned as a StringOutput.
		/// </summary>
		public sealed class DelegatedString : DelegatedValue<string, StringOutput>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.DelegatedString"/> class.
			/// </summary>
			public DelegatedString(Delegate callback)
				: base(callback, new StringOutput())
			{
			}
			
			protected override void SetValue(string value)
			{
				m_Output.Value = value;
			}
		}
		
		/// <summary>
		/// <see cref="L20nCore.Objects.DelegatedBoolean"/> represents a callback to be used
		/// at translation time, returning a bool, which will be rerturned as a BooleanValue.
		/// </summary>
		public sealed class DelegatedBoolean : DelegatedValue<bool, BooleanValue>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.DelegatedString"/> class.
			/// </summary>
			public DelegatedBoolean(Delegate callback)
				: base(callback, new BooleanValue())
			{
			}
			
			protected override void SetValue(bool value)
			{
				m_Output.Value = value;
			}
		}
	}
}
