// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

namespace L20nCore
{
	namespace IO
	{
		namespace AST
		{
			namespace L10n
			{
				/// <summary>
				/// The AST representation for a HashValue.
				/// More Information: <see cref="L20nCore.IO.Parsers.HashValue"/>
				/// </summary>
				public sealed class HashValue : INode
				{
					public HashValue()
					{
						m_Items = new Dictionary<string, INode>();
						m_Default = null;
					}

					public void AddItem(Item item)
					{
						try
						{
							m_Items.Add(item.Identifier, item.Value);
						} catch (ArgumentException)
						{
							throw new Exceptions.ImportException(
							String.Format(
							"<hash_item> with identifier {0} can't be added, as <identifier> isn't unique",
							item.Identifier));
						}
					
						if (item.IsDefault)
						{
							if (m_Default != null)
							{
								throw new Exceptions.ImportException(
								String.Format("<hash_item> already has a default with key {0}", m_Default));
							}
						
							m_Default = item.Identifier;
						}
					}
				
					public Objects.L20nObject Eval()
					{
						var items = new Dictionary<string, Objects.L20nObject>(m_Items.Count);
						foreach (KeyValuePair<string, INode> entry in m_Items)
						{
							items.Add(entry.Key, entry.Value.Eval());
						}

						return new Objects.HashValue(items, m_Default).Optimize();
					}
				
					public string Display()
					{
						string str = "{";

						foreach (KeyValuePair<string, INode> entry in m_Items)
						{
							str += String.Format("{0}{1}:{2},",
						                      entry.Key == m_Default ? "*" : "",
						                      entry.Key, entry.Value.Display());
						}

						return str + "}";
					}

					private readonly Dictionary<string, INode> m_Items;
					private string m_Default;
				
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
}
