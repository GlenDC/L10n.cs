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
			/// <see cref="L20nCore.L10n.Objects.Macro"/> represents a function defined within
			/// an L10n Resource file by the translator/developer. The parameters will be
			/// bound before translation, based on the stored parameter names and the variables
			/// given by the <see cref="L20nCore.L10n.Objects.CallExpression"/>, and unbound again
			/// right after translation.
			/// </summary>
			public sealed class Macro : L10nObject
			{
				/// <summary>
				/// Returns the name of this <see cref="L20nCore.L10n.Objects.Macro"/>.
				/// </summary>
				public string Identifier
				{
					get { return m_Identifier; }
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.Macro"/> class.
				/// </summary>
				public Macro(
				string identifier, L10nObject expression, string[] parameters)
				{
					m_Parameters = parameters;
					m_Expression = expression;
					m_Identifier = identifier;
				}

				/// <summary>
				/// Can't be optimized and returns <c>this</c> instance instead.
				/// </summary>
				public override L10nObject Optimize()
				{
					return this;
				}

				/// <summary>
				/// Evaluates the macro expression body, using the stack-pushed variables,
				/// defined by the <see cref="L20nCore.L10n.Objects.CallExpression"/> calling this macro.
				/// Returns <c>null</c> in case something went wrong.
				/// </summary>
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
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
				private readonly L10nObject m_Expression;
			}
		}
	}
}
