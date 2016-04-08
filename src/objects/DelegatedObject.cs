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
		/// <see cref="L20nCore.Objects.DelegatedLiteral"/> represents a callback to be used
		/// at translation time, returning an integer, which will be rerturned as a Literal.
		/// </summary>
		public sealed class DelegatedLiteral : L20nObject
		{	
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.DelegatedLiteral"/> class.
			/// </summary>
			public DelegatedLiteral(Delegate callback)
			{
				m_Callback = callback;
				m_Output = new Literal();
			}

			/// <summary>
			/// Can't optimize, returning <c>this</c> instance instead.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// Returns the <see cref="L20nCore.Objects.Literal"/> value filled by the stored callback.
			/// Returns <c>null</c> in case the given callback is <c>null</c>.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Callback == null)
					return null;

				m_Output.Value = m_Callback();
				return m_Output;
			}
			
			public delegate int Delegate();

			private readonly Delegate m_Callback;
			// using this variable to reuse the same Literal Instance,
			// to help stay responsible with the GC
			private Literal m_Output;
		}

		/// <summary>
		/// <see cref="L20nCore.Objects.DelegatedString"/> represents a callback to be used
		/// at translation time, returning a string, which will be rerturned as a StringOutput.
		/// </summary>
		public sealed class DelegatedString : L20nObject
		{	
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.DelegatedString"/> class.
			/// </summary>
			public DelegatedString(Delegate callback)
			{
				m_Callback = callback;
				m_Output = new StringOutput();
			}

			/// <summary>
			/// Can't optimize, returning <c>this</c> instance instead.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// Returns the <see cref="L20nCore.Objects.StringOutput"/> value filled by the stored callback.
			/// Returns <c>null</c> in case the given callback is <c>null</c>.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Callback == null)
					return null;

				m_Output.Value = m_Callback();
				return m_Output;
			}
			
			public delegate string Delegate();

			private readonly Delegate m_Callback;
			// using this variable to reuse the same StringOutput Instance,
			// to help stay responsible with the GC
			private StringOutput m_Output;
		}
	}
}
