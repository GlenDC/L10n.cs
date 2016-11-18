// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace AST
			{
				/// <summary>
				/// The AST representation for an Index.
				/// More Information: <see cref="L20nCore.L10n.IO.Parsers.Index"/>
				/// </summary>
				public sealed class Index : INode
				{
					public Index(INode index)
					{
						m_Indeces = new List<INode>(1);
						AddIndex(index);
					}
				
					public void AddIndex(INode index)
					{
						m_Indeces.Add(index);
					}
				
					public L10n.Objects.L10nObject Eval()
					{
						var indeces = new Objects.L10nObject[m_Indeces.Count];
						for (int i = 0; i < indeces.Length; ++i)
							indeces [i] = m_Indeces [i].Eval();
					
						return new L10n.Objects.Index(indeces).Optimize();
					}
				
					public string Display()
					{
						if (m_Indeces.Count == 0)
							return "";

						var indeces = new string[m_Indeces.Count];
						for (int i = 0; i < indeces.Length; ++i)
							indeces [i] = m_Indeces [i].Display();

						return String.Format("[{0}]", String.Join(",", indeces));
					}

					private readonly List<INode> m_Indeces;
				}
			}
		}
	}
}
