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

using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		public sealed class Entity : L20nObject
		{	
			public bool IsPrivate
			{
				get { return m_IsPrivate; }
			}

			private readonly L20nObject m_Index;
			private readonly L20nObject m_Value;
			private readonly bool m_IsPrivate;

			public Entity(L20nObject index, bool is_private, L20nObject value)
			{
				m_Index = index;
				m_Value = value;
				m_IsPrivate = is_private;
			}

			public Entity(External.IHashValue value)
			{
				var info = new External.InfoCollector();
				value.Collect(info);

				m_Index = null;
				m_Value = info.Collect();
				m_IsPrivate = false;
			}

			public override L20nObject Optimize()
			{
				return this;
			}
			
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if(argv.Length != 0 && (argv[0] as Dummy) != null) {
					if(m_IsPrivate) {
						Logger.Warning("entity is marked as private and cannot be accessed from C#");
						return null;
					}

					if(argv.Length > 1) {
						var arguments = new L20nObject[argv.Length-1];
						for(int i = 0; i < arguments.Length; ++i)
							arguments[i] = argv[i+1];
						return this.Eval(ctx, arguments);
					} else {
						return this.Eval(ctx);
					}
				}
	
				if (m_Index != null && argv.Length == 0) {
					var index = m_Index.Eval(ctx);
					if(index == null) {
						Logger.Warning("Entity: index couldn't be evaluated");
						return index;
					}

					var identifier = index as Identifier;
					if(identifier != null) {
						var result = m_Value.Eval(ctx, identifier);
						if(result == null) {
							Logger.Warning("<Entity>: <Identifier>-index got evaluated to null");
							return null;
						}

						return result.Eval(ctx);
					}

					var property = index as PropertyExpression;
					if(property != null) {
						var result = property.Eval(ctx, this);
						if(result == null) {
							Logger.Warning("<Entity>: <PropertyExpression>-index got evaluated to null");
							return null;
						}

						return result.Eval(ctx);
					}

					Logger.Warning("couldn't evaluate entity as index was expexted to be a <property_expression>");
					return null;
				}

				return m_Value.Eval(ctx, argv);
			}
		}
	}
}
