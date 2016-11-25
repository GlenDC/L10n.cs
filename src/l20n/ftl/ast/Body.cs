// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using System.Collections.Generic;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace AST
			{
				/// <summary>
				/// The AST representation for a Body, the root FTL Element.
				/// More Information: <see cref="L20nCore.L20n.FTL.Parsers.Body"/>
				/// </summary>
				public sealed class Body
				{
					public Body()
					{
						m_Entries = new List<INode>();
					}

					public void AddEntry(INode node)
					{
						m_Entries.Add(node);
					}
					
					public string Display()
					{
						string output = "";
						for (int i = 0; i < m_Entries.Count; ++i)
						{
							output += m_Entries [i].Display();
						}

						return output;
					}

					private List<INode> m_Entries;
				}
			}
		}
	}
}
