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
			/// The <see cref="L20nCore.L10n.Objects.IdentifierExpression"/> class represents a reference
			/// to another L21nObject in the current or default <see cref="L20nCore.L10n.Internal.LocaleContext"/>.
			/// </summary>
			public sealed class IdentifierExpression : L10nObject
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.Identifier"/> class,
				/// with the given <c>value</c> used to look up the L10nObject instance.
				/// </summary>
				public IdentifierExpression(string identifier)
				{
					m_Identifier = identifier;
				}

				/// <summary>
				/// <see cref="L20nCore.L10n.Objects.Identifier"/> can't be optimized
				/// and will instead return this instance.
				/// </summary>
				public override L10nObject Optimize()
				{
					return this;
				}

				/// <summary>
				/// Tries to get the <see cref="L20nCore.L10n.Objects.Entity"/> instance based on the stored reference (name)
				/// and will return the evaluation result of the looked up object if possible.
				/// Returns <c>null</c> in case the object could not be found or the evaluation
				/// of the looked up object returned <c>null</c> itself.
				/// </summary>
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
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
}
