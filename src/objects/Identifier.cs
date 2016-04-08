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
		/// The <see cref="L20nCore.Objects.Identifier"/> class represents a reference
	 	/// to another L20nObject in the current or default <see cref="L20nCore.Internal.LocaleContext"/>.
		/// The difference with <see cref="L20nCore.Objects.IdentifierExpression"/> as that this class is used
		/// where syntactically we have a reference, but where we want the evaluate manually,
		/// by looking the <see cref="L20nCore.Objects.Entity"/> up ourselves.
		/// </summary>
		public sealed class Identifier : L20nObject
		{
			/// <summary>
			/// Returns the reference, the name of the other L20nObject.
			/// </summary>
			public string Value
			{
				get { return m_Value; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Identifier"/> class,
			/// with the given <c>value</c> used to look up the L20nObject instance.
			/// </summary>
			public Identifier(string value)
			{
				m_Value = value;
			}

			/// <summary>
			/// <see cref="L20nCore.Objects.Identifier"/> can't be optimized
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
			
			private readonly string m_Value;
		}
	}
}
