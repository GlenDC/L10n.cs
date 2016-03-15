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

using L20n.Internal;
using L20n.Utils;

namespace L20n
{
	namespace Objects
	{
		public sealed class Entity : L20nObject
		{	
			public bool IsPrivate
			{
				get { return m_IsPrivate; }
			}

			private readonly Utils.Option<L20nObject> m_Index;
			private readonly L20nObject m_Value;
			private readonly bool m_IsPrivate;

			public Entity(Utils.Option<L20nObject> index, bool is_private, L20nObject value)
			{
				m_Index = index;
				m_Value = value;
				m_IsPrivate = is_private;
			}
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if(argv.Length != 0 && argv[0].As<Dummy>().IsSet) {
					if(m_IsPrivate) {
						Logger.Warning("entity is marked as private and cannot be accessed from C#");
						return L20nObject.None;
					}

					if(argv.Length > 1) {
						var arguments = new L20nObject[argv.Length-1];
						for(int i = 0; i < arguments.Length; ++i)
							arguments[i] = argv[1];
						return this.Eval(ctx, arguments);
					} else {
						return this.Eval(ctx);
					}
				}
	
				if (m_Index.IsSet && argv.Length == 0) {
					return m_Index.UnwrapAs<Index>()
						.Map((Index index) => m_Value.Eval(ctx, index))
						.Map((unwrapped) => unwrapped.Eval(ctx));
				}

				return m_Value.Eval(ctx, argv);
			}
		}
	}
}
