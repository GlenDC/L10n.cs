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
		public sealed class HashValue : Primitive
		{
			private readonly Dictionary<string, L20nObject> m_Items;
			private readonly string m_Default;

			public HashValue(Dictionary<string, L20nObject> items, string def)
			{
				m_Items = items;
				m_Default = def;
			}
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (argv.Length != 1) {
					if (m_Default == null) {
						Logger.Warning(
							"no <identifier> was given and <hash_value> has no default specified");
						return L20nObject.None;
					}

					return m_Items[m_Default].Eval(ctx);
				}
				
				return argv[0].Eval(ctx).Map((_id) => {
					var id = _id.As<Identifier>();

					L20nObject obj;
					if(!m_Items.TryGetValue(id.Value, out obj)) {
						if(m_Default == null) {
							Logger.Warning(
								"no defined <identifier> was given and <hash_value> has no default specified");
							return L20nObject.None;
						}

						obj = m_Items[m_Default];
					}

					return obj.Eval(ctx);
				});
			}
			
			public override Option<string> ToString(LocaleContext ctx, params L20nObject[] argv)
			{
				return Eval(ctx, argv)
					.Map((primitive) => {
						return primitive.As<Primitive>().ToString(ctx);
					});
			}
		}
	}
}
