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
			private readonly Option<string> m_Default;

			public HashValue(Dictionary<string, L20nObject> items, string def)
			{
				m_Items = new Dictionary<string, L20nObject>(items);
				m_Default = new Option<string>(def);

				if (m_Items.Count == 0) {
					Logger.Warning(
						"creating a hash value with no children is useless and a mistake, " +
						"this object will make any translation that makes use of it fail");
				}
			}

			public override L20nObject Optimize ()
			{
				if(m_Items.Count == 1) {
					return m_Items.Values.GetEnumerator().Current;
				}

				return this;
			}
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Items.Count == 0) {
					return L20nObject.None;
				}

				if (argv.Length != 1) {
					if (!m_Default.IsSet) {
						Logger.Warning(
							"no <identifier> was given and <hash_value> has no default specified");
						return L20nObject.None;
					}

					return m_Items[m_Default.Unwrap()].Eval(ctx);
				}
				
				return argv[0].Eval(ctx).UnwrapAs<Identifier>().Map((id) => {
					L20nObject obj;
					if(!m_Items.TryGetValue(id.Value, out obj)) {
						if(!m_Default.IsSet) {
							Logger.WarningFormat(
								"{0} is not a valid <identifier>, " +
								"and this <hash_value> has no default specified", id.Value);
							return L20nObject.None;
						}

						obj = m_Items[m_Default.Unwrap()];
					}

					return new Option<L20nObject>(obj);
				});
			}
			
			public override Option<string> ToString(LocaleContext ctx, params L20nObject[] argv)
			{
				return Eval(ctx, argv)
					.UnwrapAs<Primitive>().Map(
						(primitive) => primitive.ToString(ctx));
			}
		}
	}
}
