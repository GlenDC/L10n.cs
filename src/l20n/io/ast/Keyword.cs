// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

namespace L20nCore
{
	namespace L20n
	{
		namespace IO
		{
			namespace AST
			{
				/// <summary>
				/// The AST representation for a Keyword.
				/// More Information: <see cref="L20nCore.L20n.IO.Parsers.Keyword"/>
				/// </summary>
				public sealed class Keyword : INode
				{
					public Keyword(string value)
					{
						m_Value = value;
					}
					
					public L20n.Objects.L20nObject Eval()
					{
						throw new NotImplementedException();
					}
					
					public string Display()
					{
						return m_Value;
					}
					
					private readonly string m_Value;
				}
			}
		}
	}
}
