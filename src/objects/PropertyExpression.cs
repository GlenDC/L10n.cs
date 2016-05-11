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
using System.Collections.Generic;

using L20nCore.Utils;
using L20nCore.Internal;
using L20nCore.Exceptions;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// <see cref="L20nCore.Objects.PropertyExpression"/> represents a series of nested identifiers,
		/// meaning that we're looking for a value within 1 or multiple HashTables, starting with the name
		/// of the root <see cref="L20nCore.Objects.Entity"/>. 
		/// </summary>
		public sealed class PropertyExpression : L20nObject
		{
			/// <summary>
			/// Returns the series of nested identifiers,
			/// that is used by this <see cref="L20nCore.Objects.PropertyExpression"/>
			/// to look up the value in the (series of) hashtable(s).
			/// </summary>
			public L20nObject[] Identifiers
			{
				get { return m_Identifiers; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.PropertyExpression"/> class.
			/// An exception gets thrown in case less than 2 identifiers is given,
			/// as that should never happen.
			/// </summary>
			public PropertyExpression(L20nObject[] identifiers)
			{
				m_Identifiers = identifiers;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.PropertyExpression"/> class.
			/// An exception gets thrown in case less than 2 identifiers is given,
			/// as that should never happen.
			/// </summary>
			/// <param name="identifiers">Identifiers.</param>
			public PropertyExpression(string[] identifiers)
			{
				if (identifiers.Length < 2)
				{
					throw new EvaluateException("a property needs at least 2 identifiers");
				}

				// create an Identifier for each given identifier string.
				m_Identifiers = new L20nObject[identifiers.Length];
				for (int i = 0; i < identifiers.Length; ++i)
				{
					m_Identifiers [i] = new Identifier(identifiers [i]);
				}
			}

			/// <summary>
			/// Can't be optimized and will return this instance instead.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// Evaluates to the final value, found based on the stored (and evaluated) property identifiers,
			/// and optionally the given parameters.
			/// Returns null inc ase something went wrong.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				L20nObject maybe;
				int i = 0;

				// The root entity is either given as a parameter
				// or we need to get it using the first identifier.
				// See this.GetEntity for more information.
				if (argv == null || argv.Length == 0 || argv [0] as Dummy != null)
				{
					maybe = GetEntity(ctx, m_Identifiers [i]);
					i += 1;
				} else
				{
					maybe = argv [0];
				}

				// return null in case we have no root entity at all
				if (maybe == null)
				{
					Logger.Warning("<PropertyExpression>: couldn't evaluate first expression");
					return maybe;
				}

				L20nObject obj = maybe;

				// in case this property is pointing to a root entity,
				// directly requested by the L20n-user, we need to
				// evaluate it using the current identifier, such that
				// we can return null in case the Entity is private
				// and should thus not be requested.
				if (argv.Length == 1 && i < m_Identifiers.Length && (argv [0] as Dummy) != null)
				{
					obj = obj.Eval(ctx, argv [0], m_Identifiers [i]);
					++i;
				}

				// go through each identifier and replace the current Entity,
				// such that we can go deeper until we went through the
				// entire identifier list.
				for (; i < m_Identifiers.Length; ++i)
				{
					if (obj == null)
					{
						Logger.WarningFormat(
							"<PropertyExpression>: couldn't evaluate expression #{0}", i);
						return obj;
					}

					// check if we are dealing with a string
					// in which case we want to make it into an identifierExpression
					var stringObject = obj as StringOutput;
					if (stringObject != null)
					{
						// we do want to fall back to it being just a regular string,
						// in case that was the actual intention of the user
						L20nObject alt = new IdentifierExpression(stringObject.Value);
						alt = alt.Eval(ctx, m_Identifiers [i]);
						if (alt != null)
						{
							obj = alt;
							continue;
						}
					}

					obj = obj.Eval(ctx, m_Identifiers [i]);
				}

				if (obj == null)
				{
					Logger.Warning(
						"<PropertyExpression>: couldn't evaluate the final expression");
					return obj;
				}

				// if all is fine, we can now return the evaluation
				// of the last found value.
				return obj.Eval(ctx);
			}

			/// <summary>
			/// A help function to get the root entity based on the first variable.
			/// The given identifier is either an Identifier, Variable or Global,
			/// which will define the action to be taken in order to get and return the Root Entity.
			/// </summary>
			private Entity GetEntity(LocaleContext ctx, L20nObject key)
			{
				// is it an identifier?
				var identifier = key as Identifier;
				if (identifier != null)
					return ctx.GetEntity(identifier.Value);

				// is it a variable?
				var variable = key as Variable;
				if (variable != null)
				{
					var obj = ctx.GetVariable(variable.Identifier);
					
					// a variable can also be a string-reference to an entity
					var reference = obj as StringOutput;
					if (reference != null)
					{
						return ctx.GetEntity(reference.Value);
					}

					// otherwise it's simply an entity (or at least it should be)
					return obj as Entity;
				}

				// is it a global?
				var global = key as Global;
				if (global != null)
					return ctx.GetGlobal(global.Identifier) as Entity;

				// is it a string?
				var str = key as StringOutput;
				if (str != null)
					return ctx.GetEntity(str.Value);

				return null;
			}
			
			private readonly L20nObject[] m_Identifiers;
		}
	}
}
