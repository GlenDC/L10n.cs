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
		/// The <see cref="L20nCore.Objects.IdentifierExpression"/> class represents a reference
		/// to another L20nObject in the current or default <see cref="L20nCore.Internal.LocaleContext"/>.
		/// </summary>
		public sealed class IdentifierExpression : L20nObject
		{	
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Identifier"/> class,
			/// with the given <c>value</c> used to look up the L20nObject instance.
			/// </summary>
			public IdentifierExpression(string identifier)
			{
				m_Identifier = identifier;
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
			/// Tries to get the <see cref="L20nCore.Objects.Entity"/> instance based on the stored reference (name) 
			/// and will return the evaluation result of the looked up object if possible.
			/// Returns <c>null</c> in case the object could not be found or the evaluation
			/// of the looked up object returned <c>null</c> itself.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				var entity = ctx.GetEntity(m_Identifier);
				if (entity == null)
				{
					return entity;
				}

				return entity.Eval(ctx, argv);
			}

			private readonly string m_Identifier;
		}
	}
}
