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
		/// <see cref="L20nCore.Objects.StringOutput"/> is a L20nObject type that
		/// represents a string value, either static or as the result of a L20n Expression.
		/// </summary>
		public sealed class StringOutput : Primitive
		{	
			/// <summary>
			/// A public interface to the actual string value that
			/// makes up this <see cref="L20nCore.Objects.StringOutput"/> object.
			/// </summary>
			public string Value
			{
				get { return m_Value; }
				set { m_Value = value; }
			}

			private string m_Value;

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.StringOutput"/> class
			/// with an undefined string value.
			/// </summary>
			public StringOutput()
			{
				m_Value = "";
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.StringOutput"/> class
			/// with an initial string value given by the callee of this constructor.
			/// </summary>
			public StringOutput(string value)
			{
				m_Value = value;
			}

			/// <summary>
			/// <see cref="L20nCore.Objects.StringOutput"/> is already the most primitive L20nType of its kind
			/// and can therefore not be further optimized and simply returns itself as a result.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// The evaluation of a <see cref="L20nCore.Objects.StringOutput"/> object
			/// is as simple as returning itself.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return this;
			}

			/// <summary>
			/// Returns the string value that makes up this
			/// <see cref="L20nCore.Objects.StringOutput"/> object. 
			/// </summary>
			public override string ToString(LocaleContext ctx, params L20nObject[] argv)
			{
				return m_Value;
			}
		}
	}
}
