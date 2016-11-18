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
			/// The <see cref="L20nCore.L10n.Objects.KeyValuePair"/> represents a single attribute
			/// containing a <see cref="L20nCore.L10n.Objects.L20nObject"/> value and optionally an index.
			/// </summary>
			public sealed class KeyValuePair : L10nObject
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.HashValue"/> class.
				/// </summary>
				public KeyValuePair(L10nObject value, L10nObject index)
				{
					m_Value = value;
					m_Index = index;
				}
			
				/// <summary>
				/// If the value can optimize, we can simply return its optimized version.
				/// Otherwise this object itself will be returned.
				/// </summary>
				public override L10nObject Optimize()
				{
					var optimizedValue = m_Value.Optimize();
					if (optimizedValue != m_Value)
					{
						return optimizedValue;
					}

					return this;
				}
			
				/// <summary>
				/// Evaluates the given value, with optionally an index given as a first argument,
				/// in case the value is a <see cref="L20nCore.L10n.Objects.HashValue"/>.
				/// If the index of this attribute is set, and no arguments are given, it will be used as the argument.
				/// Otherwise we'll simply evaluate the value with the given arguments, which can be none.
				/// </summary>
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
				{
					// if index is given and no extern parameters have been given,
					// we'll evaluate the index and will use that to evaluate the final output value.
					// this also assumes that in case the index is given,
					// that the value is a HashValue rather than a StringValue,
					// which is something the parser of this type enforces anyhow.
					if (m_Index != null && argv.Length == 0)
					{
						var index = m_Index.Eval(ctx);
						if (index == null)
						{
							Logger.Warning("<KeyValuePair>: index couldn't be evaluated");
							return index;
						}
					
						// could be one simple identifier, in which case we can evaluate
						// the HashValue of this instance given the identifier as a parameter
						var identifier = index as Identifier;
						if (identifier != null)
						{
							var result = m_Value.Eval(ctx, identifier);
							if (result == null)
							{
								Logger.Warning("<KeyValuePair>: <Identifier>-index got evaluated to null");
								return null;
							}
						
							return result.Eval(ctx);
						}
					
						// otherwise it should be a PropertyExpression,
						// in which case we evaluate that expression given this instance as the first parameter.
						var property = index as PropertyExpression;
						if (property != null)
						{
							var result = property.Eval(ctx, this);
							if (result == null)
							{
								Logger.Warning("<Entity>: <PropertyExpression>-index got evaluated to null");
								return null;
							}
						
							return result.Eval(ctx);
						}
					
						Logger.Warning("couldn't evaluate <KeyValuePair> as index was expexted to be a <property_expression>");
						return null;
					}
				
					// in all other cases we simply evaluate the wrapped value,
					// passing on the given external parameters (e.g. an index)
					return m_Value.Eval(ctx, argv);
				}

				private readonly L10nObject m_Value;
				private readonly L10nObject m_Index;
			}
		}
	}
}
