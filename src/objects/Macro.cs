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
using System.Collections.Generic;

using L20nCore.Utils;
using L20nCore.Internal;
using L20nCore.Exceptions;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// <see cref="L20nCore.Objects.Macro"/> represents a function defined within
	 	/// an L20n Resource file by the translator/developer. The parameters will be
		/// bound before translation, based on the stored parameter names and the variables
		/// given by the <see cref="L20nCore.Objects.CallExpression"/>, and unbound again
		/// right after translation.
		/// </summary>
		public sealed class Macro : L20nObject
		{	
			/// <summary>
			/// Returns the name of this <see cref="L20nCore.Objects.Macro"/>. 
			/// </summary>
			public string Identifier
			{
				get { return m_Identifier; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Macro"/> class.
			/// </summary>
			public Macro(
				string identifier, L20nObject expression, string[] parameters)
			{
				m_Parameters = parameters;
				m_Expression = expression;
				m_Identifier = identifier;
			}

			/// <summary>
			/// Can't be optimized and returns <c>this</c> instance instead.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// Evaluates the macro expression body, using the stack-pushed variables,
			/// defined by the <see cref="L20nCore.Objects.CallExpression"/> calling this macro.
			/// Returns <c>null</c> in case something went wrong.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Parameters.Length != argv.Length)
				{
					Logger.WarningFormat(
						"<macro> expects {0} parameters, received {1}",
						m_Parameters.Length, argv.Length);
					return null;
				}

				// Push variables on 'stack', parameters with previously used names will be shadowed.
				for (int i = 0; i < m_Parameters.Length; ++i)
				{
					ctx.PushVariable(m_Parameters [i], argv [i].Eval(ctx));
				}

				// evaluate the actual macro expression
				var output = m_Expression.Eval(ctx);

				// Remove them from the 'stack'
				for (int i = 0; i < m_Parameters.Length; ++i)
				{
					ctx.DropVariable(m_Parameters [i]);
				}

				return output;
			}
			
			private readonly string m_Identifier;
			private readonly string[] m_Parameters;
			private readonly L20nObject m_Expression;
		}
	}
}
