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
		/// <see cref="L20nCore.Objects.Literal"/> is a L20nObject type that
		/// represents a fixed whole number. This can be either a static number as given
		/// in an L20n Language File, or the result of a computation.
		/// </summary>
		public sealed class Literal : Primitive
		{
			/// <summary>
			/// A public interface to the actual integer value that
			/// makes up this <see cref="L20nCore.Objects.Literal"/> object.
			/// </summary>
			public int Value
			{
				get { return m_Value; }
				set { m_Value = value; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Literal"/> class
			/// with an undefined integer value.
			/// </summary>
			public Literal()
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Literal"/> class
			/// with a defined initial integer value given by callee of this constructor.
			/// </summary>
			public Literal(int value)
			{
				m_Value = value;
			}

			/// <summary>
			/// <see cref="L20nCore.Objects.Literal"/> is already the most primitive L20nType of its kind
			/// and can therefore not be further optimized and simply returns itself as a result.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// The evaluation of a <see cref="L20nCore.Objects.Literal"/> object
			/// is as simple as returning itself.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return this;
			}

			/// <summary>
			/// Returns the string representation of the <see cref="System.Int32"/> value
			/// contained in this <see cref="L20nCore.Objects.Literal"/> object. 
			/// </summary>
			public override string ToString(LocaleContext ctx, params L20nObject[] argv)
			{
				return m_Value.ToString();
			}
			
			private int m_Value;
		}
	}
}
