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
	namespace IO
	{
		namespace AST
		{
			public sealed class HashValue : INode
			{
				private Dictionary<string, INode> m_Items;
				private string m_Default;

				public HashValue()
				{
					m_Items = new Dictionary<string, INode>();
					m_Default = null;
				}

				public void AddItem(Item item)
				{
					try {
						m_Items.Add(item.Identifier, item.Value);
					}
					catch(ArgumentException) {
						throw new L20n.Exceptions.ImportException(
							String.Format(
							"<hash_item> with identifier {0} can't be added, as <identifier> isn't unique",
							item.Identifier));
					}
					
					if(item.IsDefault) {
						if(m_Default != null) {
							throw new L20n.Exceptions.ImportException(
								String.Format("<hash_item> already has a default with key {0}", m_Default));
						}
						
						m_Default = item.Identifier;
					}
				}
				
				public L20n.Objects.L20nObject Eval()
				{
					var items = new Dictionary<string, L20n.Objects.L20nObject> (m_Items.Count);
					foreach (KeyValuePair<string, INode> entry in m_Items) {
						items.Add(entry.Key, entry.Value.Eval());
					}

					return new L20n.Objects.HashValue(items, m_Default);
				}
				
				public string Display()
				{
					string str = "{";

					foreach (KeyValuePair<string, INode> entry in m_Items) {
						str += String.Format ("{0}{1}:{2}",
						                      entry.Key == m_Default ? "*" : "",
						                      entry.Key, entry.Value.Display ());
					}

					return str + "}";
				}
				
				public class Item
				{
					public string Identifier { get; private set; }
					public INode Value { get; private set; }
					public bool IsDefault { get; private set; }
					
					public Item(string id, INode val, bool is_def)
					{
						Identifier = id;
						Value = val;
						IsDefault = is_def;
					}
				}
			}
		}
	}
}
