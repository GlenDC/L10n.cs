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
		/// The <see cref="L20nCore.Objects.HashValue"/> represents a hash table
		/// containing <see cref="L20nCore.Objects.L20nObject"/> values matched with a hash.
		/// A value can be looked based on a found given hash key or the default value in case that is specified.
		/// </summary>
		public sealed class HashValue : Primitive
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.HashValue"/> class.
			/// </summary>
			public HashValue(Dictionary<string, L20nObject> items, string def)
			{
				m_Items = new Dictionary<string, L20nObject>(items);
				m_Default = def;

				if (m_Items.Count == 0)
				{
					Logger.Warning(
						"creating a hash value with no children is useless and a mistake, " +
						"this object will make any translation that makes use of it fail");
				}
			}

			/// <summary>
			/// Optimizes to a simple <see cref="L20nCore.Objects.L20nObject"/> value in case
			/// only one value is define within the given HashTable.
			/// Returns this instance oterhwise.
			/// </summary>
			public override L20nObject Optimize()
			{
				if (m_Items.Count == 1)
				{
					var enumerator = m_Items.Values.GetEnumerator();
					enumerator.MoveNext();
					return enumerator.Current;
				}

				return this;
			}

			/// <summary>
			/// Returns the value that matches the hash key given as the first given parameter.
			/// In case a default value is specified, and no hash key is given or the given hash key
			/// matches no key registered in the hash table, that value will be returned instead.
			/// In all other cases, including when something went wrong, <c>null</c> will be returned.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Items.Count == 0)
				{
					return null;
				}

				if (argv.Length != 1)
				{
					if (m_Default == null)
					{
						Logger.Warning(
							"no <identifier> was given and <hash_value> has no default specified");
						return null;
					}

					return m_Items [m_Default].Eval(ctx);
				}

				var first = argv [0].Eval(ctx);

				Identifier id = first as Identifier;
				if (id == null)
				{
					var str = first as StringOutput;
					if (str == null)
					{
						Logger.Warning("HashValue: first variadic argument couldn't be evaluated as an <Identifier>");
						return id;
					}

					id = new Identifier(str.Value);
				}	

				L20nObject obj;
				if (!m_Items.TryGetValue(id.Value, out obj))
				{
					if (m_Default == null)
					{
						Logger.WarningFormat(
							"{0} is not a valid <identifier>, " +
							"and this <hash_value> has no default specified", id.Value);
						return null;
					}

					obj = m_Items [m_Default];
				}

				return obj;
			}

			/// <summary>
			/// Evaluates this instance and returns the resulting primitive to a string value if possible.
			/// Returns <c>null</c> in case something went wrong during evaluation or if the evaluation result is
			/// not a <see cref="L20nCore.Objects.Primitive"/> value type.
			/// </summary>
			public override string ToString(LocaleContext ctx, params L20nObject[] argv)
			{
				var primitive = Eval(ctx, argv) as Primitive;
				if (primitive != null)
					return primitive.ToString(ctx);

				return null;
			}

			private readonly Dictionary<string, L20nObject> m_Items;
			private readonly string m_Default;
		}
	}
}
