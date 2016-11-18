// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Utils;

using L20nCore.L10n.Internal;

namespace L20nCore
{
	namespace L10n
	{
		namespace Objects
		{
			/// <summary>
			/// The <see cref="L20nCore.L10n.Objects.Variable"/> class defines the reference that will look up 
			/// an external variable given by the user within the same translation call where it is used.
			/// </summary>
			public sealed class Variable : L10nObject
			{	
				/// <summary>
				/// Returns the string reference that this Variable will look up.
				/// </summary>
				public string Identifier
				{
					get { return m_Identifier; }
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.Variable"/> class 
				/// with the given identifier as the reference to look up during translation.
				/// </summary>
				public Variable(string identifier)
				{
					m_Identifier = identifier;
				}

				/// <summary>
				/// The <see cref="L20nCore.L10n.Objects.Variable"/> class can't be optimized
				/// and will simply return this instance. 
				/// </summary>
				public override L10nObject Optimize()
				{
					return this;
				}

				/// <summary>
				/// Tries to get the external variable based on the stored reference
				/// and will return the evaluation result of the looked up object if possible.
				/// Returns <c>null</c> in case the variable could not be found or the evaluation
				/// of the looked up variable returned <c>null</c>.
				/// </summary>
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
				{
					var variable = ctx.GetVariable(m_Identifier);
					if (variable == null)
					{
						return variable;
					}

					return variable.Eval(ctx, argv);
				}
			
				private readonly string m_Identifier;
			}
		}
	}
}
