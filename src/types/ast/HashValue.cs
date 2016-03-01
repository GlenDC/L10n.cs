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

namespace L20n
{
	namespace Types
	{
		namespace AST
		{
			public class HashValue : Value
			{
				private List<HashItem> m_Items;

				public HashValue()
				{
					m_Items = new List<HashItem>();
				}

				public void AddItem(HashItem item)
				{
					m_Items.Add(item);
				}

				public override bool Evaluate(out Internal.Expressions.Value output)
				{
					var table = new Dictionary<string, Types.Internal.Expressions.Primary>(m_Items.Count);
					Types.Internal.Expressions.Primary defaultItem = null;

					foreach(var item in m_Items) {
						// make sure we don't have duplicates
						if(table.ContainsKey(item.Identifier)) {
							string msg = String.Format(
								"unexpected <identifier> {0}, <hash_value> can not contain a duplicate <identifier>",
								item.Identifier);
							throw new L20n.Exceptions.EvaluateException(msg);
						}

						// add to table
						table.Add(item.Identifier, item.Value);

						// mark as default if we don't have one yet
						if(item.IsDefault) {
							if(defaultItem != null) {
								string msg = String.Format(
									"unexpected default <hash_item> {0}, <hash_value> can only contain one default <hash_item>",
									item.Identifier);
								throw new L20n.Exceptions.EvaluateException(msg);
							}

							defaultItem = item.Value;
						}
					}

					if (table.Count != 0) {
						output = new Internal.Expressions.HashValue(table, defaultItem);
						return true;
					}

					output = null;
					return false;
				}
			}
		}
	}
}

