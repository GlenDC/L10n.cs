// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20nCore.Exceptions;

namespace L20nCore
{
	namespace IO
	{
		namespace AST
		{
			namespace L10n
			{
				/// <summary>
				/// The AST representation for an Index.
				/// More Information: <see cref="L20nCore.IO.Parsers.Index"/>
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
				
					public Objects.L20nObject Eval()
					{
						var indeces = new Objects.L20nObject[m_Indeces.Count];
						for (int i = 0; i < indeces.Length; ++i)
							indeces [i] = m_Indeces [i].Eval();
					
						return new Objects.Index(indeces).Optimize();
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
