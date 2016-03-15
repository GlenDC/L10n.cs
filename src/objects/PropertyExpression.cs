/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 - 2017 Glen De Cauwsemaecker (contact@glendc.com)
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

using L20n.Utils;
using L20n.Internal;
using L20n.Exceptions;

namespace L20n
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
				if (identifiers.Length < 2) {
					throw new ParseException("a property needs at least 2 identifiers");
				}
	
				m_Identifiers = identifiers;
			}
			
			public PropertyExpression(string[] identifiers)
			{
				if (identifiers.Length < 2) {
					throw new EvaluateException("a property needs at least 2 identifiers");
				}

				m_Identifiers = new L20nObject[identifiers.Length];
				for (int i = 0; i < identifiers.Length; ++i) {
					m_Identifiers [i] = new Identifier (identifiers [i]);
				}
			}
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{	
				return GetEntity(ctx, m_Identifiers[0]).Map((entity) => {
					var obj = new Option<L20nObject>(entity);

					int i = 1;
					if(argv.Length == 1 && argv[0].As<Dummy>().IsSet) {
						obj = obj.Map((unwrapped) =>
						              unwrapped.Eval(ctx, argv[0], m_Identifiers[i]));
						++i;
					}

					for(; i < m_Identifiers.Length; ++i) {
						obj = obj.Map((unwrapped) =>
						              unwrapped.Eval(ctx, m_Identifiers[i]));
					}

					return obj.Map((unwrapped) => unwrapped.Eval(ctx));
				});
			}

			private Option<Entity> GetEntity(LocaleContext ctx, L20nObject key)
			{
				Option<Entity> wrapped;

				// is it an identifier?
				wrapped = key.As<Identifier>()
					.Map((identifier) => ctx.GetEntity(identifier.Value));

				if(wrapped.IsSet) return wrapped;

				// is it a variable?
				wrapped = key.As<Variable>()
					.Map((variable) => ctx.GetVariable(variable.Identifier))
					.UnwrapAs<Entity>();
				
				if (wrapped.IsSet) return wrapped;

				// ok so it better be a global
				return key.As<Global>()
					.Map((global) => ctx.GetGlobal(global.Identifier))
					.UnwrapAs<Entity>();

			}
		}
	}
}
