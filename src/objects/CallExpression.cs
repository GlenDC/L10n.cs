// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// The <see cref="L20nCore.Objects.CallExpression"/> represents the call to
		/// a <see cref="L20nCore.Objects.Macro"/>, meaning that it will look up the requested macro based on
		/// a stored identifier reference and will than evaluate it by passing it in
		/// the values to be bound the the macro parameters during evaluation of that macro.
		/// </summary>
		public sealed class CallExpression : L20nObject
		{	
			public CallExpression(string identifier, L20nObject[] variables)
			{
				m_Identifier = identifier;
				m_Variables = variables;
			}

			/// <summary>
			/// Can't be optimized and returns this instance instead.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// Looks up the Macro in the current or default <see cref="L20nCore.Internal.LocaleContext"/>
			/// and returns the evaluation of the macro evaluation passing in the values as parameters,
			/// so that they can be bound to the macro parameters during the actual evaluation.
			/// Returns <c>null</c> in case the macro couldn't be found, or in case the Macro Evaluation
			/// itself returned <c>null</c>.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				var macro = ctx.GetMacro(m_Identifier);
				if (macro == null)
				{
					Logger.WarningFormat("couldn't find macro with name {0}", m_Identifier);
					return macro;
				}

				return macro.Eval(ctx, m_Variables);
			}

			private readonly L20nObject[] m_Variables;
			private readonly string m_Identifier;
		}
	}
}
