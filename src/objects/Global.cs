// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// The <see cref="L20nCore.Objects.Global"/> class represents a reference
		/// to a global L20nObject.
		/// </summary>
		public sealed class Global : L20nObject
		{	
			/// <summary>
			/// Returns the reference, the name of the global L20nObject.
			/// </summary>
			public string Identifier
			{
				get { return m_Identifier; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Global"/> class,
			/// with the given <c>value</c> used to look the L20nObject global.
			/// </summary>
			public Global(string identifier)
			{
				m_Identifier = identifier;
			}

			public override L20nObject Optimize()
			{
				return this;
			}

			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
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
