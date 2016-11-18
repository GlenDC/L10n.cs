// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20nCore.Common;
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
			/// <see cref="L20nCore.L10n.Objects.Index"/> represents the optional index
			/// (an identifier or propertyExpression) that can be defined for an Entity, which can be used,
			/// in case we need an expression to define the default (nested) value of a HashTable.
			/// </summary>
			public sealed class Index : L10nObject
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.Index"/> class.
				/// </summary>
				public Index(L10nObject[] indeces)
				{
					m_Indeces = indeces;
					m_EvaluatedIndeces = new L10nObject[indeces.Length];
				}

				/// <summary>
				/// Can't optimize, returns <c>this</c> instance instead.
				/// </summary>
				public override L10nObject Optimize()
				{
					// relies on ctx use, so no way to optimize this
					return this;
				}

				/// <summary>
				/// Evaluates <c>this</c> instance to an Identifier or PropertyExpression.
				/// Returns <c>null</c> in case something went wrong.
				/// </summary>
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
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
								Logger.WarningFormat(
								"index #{0} got evaluated to null, stopping the evaluation of this Index", i);
							} else
							{
								Logger.WarningFormat(
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

				private readonly L10nObject[] m_Indeces;
				private L10nObject[] m_EvaluatedIndeces;
			}
		}
	}
}
