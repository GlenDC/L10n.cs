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
			/// The <see cref="L20nCore.L10n.Objects.Global"/> class represents a reference
			/// to a global L10nObject.
			/// </summary>
			public sealed class Global : L10nObject
			{	
				/// <summary>
				/// Returns the reference, the name of the global L10nObject.
				/// </summary>
				public string Identifier
				{
					get { return m_Identifier; }
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.Global"/> class,
				/// with the given <c>value</c> used to look the L10nObject global.
				/// </summary>
				public Global(string identifier)
				{
					m_Identifier = identifier;
				}

				public override L10nObject Optimize()
				{
					return this;
				}

				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
				{
					var global = ctx.GetGlobal(m_Identifier);
					if (global == null)
					{
						return global;
					}

					return global.Eval(ctx, argv);
				}
			
				private readonly string m_Identifier;
			}
		}
	}
}
