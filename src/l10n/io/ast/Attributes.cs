// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace AST
			{
				/// <summary>
				/// The AST representation for a Attributes.
				/// More Information: <see cref="L20nCore.L10n.IO.Parsers.Attributes"/>
				/// </summary>
				public sealed class Attributes : INode
				{
					public Attributes()
					{
						m_Items = new Dictionary<string, Item>();
					}
				
					public void AddItem(Item item)
					{
						if (item == null)
						{
							throw new ImportException(
							String.Format(
							"<attribute> can't be added as a null-value",
							item.Identifier));
						}

						try
						{
							m_Items.Add(item.Identifier, item);
						} catch (ArgumentException)
						{
							throw new ImportException(
							String.Format(
							"<attribute> with identifier {0} can't be added, as <identifier> isn't unique",
							item.Identifier));
						}
					}
				
					public L10n.Objects.L10nObject Eval()
					{
						var items = new Dictionary<string, L10n.Objects.L10nObject>(m_Items.Count);
						foreach (KeyValuePair<string, Item> entry in m_Items)
						{
							var value = entry.Value.Eval();
							if (value == null)
							{
								throw new ImportException(
								String.Format(
								"<attribute> with id {0} can't be created as evaluated value is null",
								entry.Key));
							}

							items.Add(entry.Key, value);
						}
					
						return new L10n.Objects.Attributes(items).Optimize();
					}
				
					public string Display()
					{
						string str = "";
						foreach (KeyValuePair<string, Item> entry in m_Items)
						{
							str += entry.Value.Display();
						}
					
						return str;
					}
				
					private readonly Dictionary<string, Item> m_Items;
				
					public class Item : INode
					{
						public string Identifier { get; private set; }
					
						private INode m_Index;
						private INode m_Value;
					
						public Item(string id, INode index, INode val)
						{
							if (val == null)
							{
								throw new ImportException(
								String.Format(
								"<attribute> with id {0} can't be created as value is null",
								id));
							}

							Identifier = id;
							m_Index = index;
							m_Value = val;
						}

						public L10n.Objects.L10nObject Eval()
						{
							L10n.Objects.L10nObject index =
							m_Index == null ? null : m_Index.Eval();
							var value = m_Value.Eval();

							if (value == null)
							{
								return null;
							}

							return new L10n.Objects.KeyValuePair(value, index);
						}
					
						public string Display()
						{
							return String.Format(" {0}{1}: {2}",
							Identifier,
						    m_Index != null ? m_Index.Display() : "",
						    m_Value.Display());
						} 
					}
				}
			}
		}
	}
}
