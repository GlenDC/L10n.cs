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
		/// The <see cref="L20nCore.Objects.BooleanValue"/> class represents a constant boolean value.
		/// </summary>
		public sealed class BooleanValue : L20nObject
		{
			/// <summary>
			/// A public interface to the actual boolean value that
			/// makes up this <see cref="L20nCore.Objects.BooleanValue"/> object.
			/// </summary>
			public bool Value
			{
				get { return m_Value; }
				set { m_Value = value; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.BooleanValue"/> class
			/// with an undefined boolean value.
			/// </summary>
			public BooleanValue()
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.BooleanValue"/> class
			/// wrapping up the given boolean value.
			/// </summary>
			public BooleanValue(bool value)
			{
				m_Value = value;
			}

			/// <summary>
			/// <see cref="L20nCore.Objects.BooleanValue"/> can't be optimized
			/// and will instead return this instance.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// Returns simply this instance, and no actual evaluation takes place.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return this;
			}
			
			private bool m_Value;
		}
	}
}
