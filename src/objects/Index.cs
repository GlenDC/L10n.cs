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
		/// <see cref="L20nCore.Objects.Index"/> represents the optional index
		/// (an identifier or propertyExpression) that can be defined for an Entity, which can be used,
		/// in case we need an expression to define the default (nested) value of a HashTable.
		/// </summary>
		public sealed class Index : L20nObject
		{	
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Index"/> class.
			/// </summary>
			public Index(L20nObject[] indeces)
			{
				m_Indeces = indeces;
				m_EvaluatedIndeces = new L20nObject[indeces.Length];
			}

			/// <summary>
			/// Can't optimize, returns <c>this</c> instance instead.
			/// </summary>
			public override L20nObject Optimize()
			{
				// relies on ctx use, so no way to optimize this
				return this;
			}

			/// <summary>
			/// Evaluates <c>this</c> instance to an Identifier or PropertyExpression.
			/// Returns <c>null</c> in case something went wrong.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				// Evaluates each stored index and expects them to be Identifiers
				// (or strings that can be converted to Identifiers)
				for (int i = 0; i < m_EvaluatedIndeces.Length; ++i)
				{
					var index = m_Indeces [i].Eval(ctx);
					var identifier = index as Identifier;
					if (identifier == null)
					{
						var output = index as StringOutput;
						if (output != null)
							identifier = new Identifier(output.Value);
					}

					// if it's not an identifier, we simply return null,
					// as something went wrong.
					if (identifier != null)
					{
						m_EvaluatedIndeces [i] = identifier;
					} else
					{
						if (index == null)
						{
							Internal.Logger.WarningFormat(
								"index #{0} got evaluated to null, stopping the evaluation of this Index", i);
						} else
						{
							Internal.Logger.WarningFormat(
								"index #{0} got evaluated to {1}, while expecting {2}," +
								"stopping the evaluation of this Index", i, index.GetType(), typeof(Identifier));
						}

						return null;
					}
				}

				// we either have a simple Identifier, or we need to generate
				// a property expressions based on the given Indeces.
				if (m_EvaluatedIndeces.Length == 1)
					return m_EvaluatedIndeces [0];

				return new PropertyExpression(m_EvaluatedIndeces);
			}

			private readonly L20nObject[] m_Indeces;
			private L20nObject[] m_EvaluatedIndeces;
		}
	}
}
