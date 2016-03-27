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
		public sealed class PropertyExpression : L20nObject
		{
			public L20nObject[] Identifiers
			{
				get { return m_Identifiers; }
			}

			private readonly L20nObject[] m_Identifiers;
			
			public PropertyExpression(L20nObject[] identifiers)
			{
				if (identifiers.Length < 2)
				{
					throw new ParseException("a property needs at least 2 identifiers");
				}
	
				m_Identifiers = identifiers;
			}
			
			public PropertyExpression(string[] identifiers)
			{
				if (identifiers.Length < 2)
				{
					throw new EvaluateException("a property needs at least 2 identifiers");
				}

				m_Identifiers = new L20nObject[identifiers.Length];
				for (int i = 0; i < identifiers.Length; ++i)
				{
					m_Identifiers [i] = new Identifier(identifiers [i]);
				}
			}

			public override L20nObject Optimize()
			{
				return this;
			}
			
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				Entity maybe;
				int i = 0;

				if (argv == null || argv.Length == 0 || (argv [0] as Entity) == null)
				{
					maybe = GetEntity(ctx, m_Identifiers [i]);
					i += 1;
				} else
				{
					maybe = argv [0] as Entity;
				}

				if (maybe == null)
				{
					Logger.Warning("<PropertyExpression>: couldn't evaluate first expression");
					return maybe;
				}

				L20nObject obj = maybe;

				if (argv.Length == 1 && (argv [0] as Dummy) != null)
				{
					obj = obj.Eval(ctx, argv [0], m_Identifiers [i]);
					++i;
				}

				for (; i < m_Identifiers.Length; ++i)
				{
					if (obj == null)
					{
						Logger.WarningFormat(
							"<PropertyExpression>: couldn't evaluate expression #{0}", i);
						return obj;
					}

					obj = obj.Eval(ctx, m_Identifiers [i]);
				}

				if (obj == null)
				{
					Logger.Warning(
						"<PropertyExpression>: couldn't evaluate the final expression");
					return obj;
				}

				return obj.Eval(ctx);
			}

			private Entity GetEntity(LocaleContext ctx, L20nObject key)
			{
				// is it an identifier?
				var identifier = key as Identifier;
				if (identifier != null)
					return ctx.GetEntity(identifier.Value);

				// is it a variable?
				var variable = key as Variable;
				if (variable != null)
					return ctx.GetVariable(variable.Identifier) as Entity;

				// is it a global?
				var global = key as Global;
				if (global != null)
					return ctx.GetGlobal(global.Identifier) as Entity;

				return null;
			}
		}
	}
}
