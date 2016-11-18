// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common;
using L20nCore.Common.Utils;

using L20nCore.L10n.Internal;

namespace L20nCore
{
	namespace L10n
	{
		namespace Objects
		{
			/// <summary>
			/// The <see cref="L20nCore.L10n.Objects.AttributeExpression"/> class represents a reference
			/// to an atribute within the referenced etity
			/// in the current or default <see cref="L20nCore.L10n.Internal.LocaleContext"/>.
			/// </summary>
			public sealed class AttributeExpression : L10nObject
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.Identifier"/> class,
				/// with the given <c>value</c> used to look up the L10nObject instance.
				/// </summary>
				public AttributeExpression(L10nObject root, L10nObject identifier, L10nObject propertyExpression)
				{
					m_Root = root;
					m_Identifier = identifier;
					m_PropertyExpression = propertyExpression;
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
					var entity = GetEntity(ctx);
					if (entity == null)
					{
						Logger.Warning("AttributeExpression: couldn't find the entity");
						return entity;
					}

					var identifier = GetIdentifier(ctx);
					if (identifier == null)
					{
						Logger.Warning("AttributeExpression: couldn't evaluate identifier");
						return identifier;
					}

					var attribute = entity.GetAttribute(ctx, identifier);
					if (attribute == null)
					{
						Logger.WarningFormat("AttributeExpression: couldn't find an atttribute with key {0}", identifier.Value);
						return attribute;
					}

					// mixed expressions are valid too
					if (m_PropertyExpression != null)
					{
						return m_PropertyExpression.Eval(ctx, attribute);
					}

					return attribute.Eval(ctx);
				}

				/// <summary>
				/// A helper function to retrieve the identifier,
				/// which can either be an identifier already or a string value,
				/// which has to be transformed to an identifier at runtime.
				/// </summary>
				private Identifier GetIdentifier(LocaleContext ctx)
				{
					var obj = m_Identifier.Eval(ctx);

					// check if it's a string value
					var stringOutput = obj as StringOutput;
					if (stringOutput != null)
					{
						return new Identifier(stringOutput.Value);
					}

					// otherwise simply return the object `as` an `Identifier`
					return obj as Identifier;
				}

				/// <summary>
				/// A help function to get the entity based on the first variable.
				/// The given identifier is either an Identifier, Variable or Global,
				/// which will define the action to be taken in order to get and return the Root Entity.
				/// </summary>
				private Entity GetEntity(LocaleContext ctx)
				{
					// is it an identifier?
					var identifier = m_Root as Identifier;
					if (identifier != null)
						return ctx.GetEntity(identifier.Value);

					// is it a string?
					var str = m_Root.Eval(ctx) as StringOutput;
					if (str != null)
						return ctx.GetEntity(str.Value);

					// is it a variable?
					var variable = m_Root as Variable;
					if (variable != null)
						return ctx.GetVariable(variable.Identifier) as Entity;

					// is it a global?
					var global = m_Root as Global;
					if (global != null)
						return ctx.GetGlobal(global.Identifier) as Entity;

					return null;
				}

				private readonly L10nObject m_PropertyExpression;
				private readonly L10nObject m_Identifier;
				private readonly L10nObject m_Root;
			}
		}
	}
}
