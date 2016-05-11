/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 Glen De Cauwsemaecker (contact@glendc.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// The <see cref="L20nCore.Objects.AttributeExpression"/> class represents a reference
		/// to an atribute within the referenced etity
		/// in the current or default <see cref="L20nCore.Internal.LocaleContext"/>.
		/// </summary>
		public sealed class AttributeExpression : L20nObject
		{	
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Identifier"/> class,
			/// with the given <c>value</c> used to look up the L20nObject instance.
			/// </summary>
			public AttributeExpression(L20nObject root, L20nObject identifier, L20nObject propertyExpression)
			{
				m_Root = root;
				m_Identifier = identifier;
				m_PropertyExpression = propertyExpression;
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
			
			private readonly L20nObject m_PropertyExpression;
			private readonly L20nObject m_Identifier;
			private readonly L20nObject m_Root;
		}
	}
}
