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
