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
				/// The AST representation for a Comment.
				/// More Information: <see cref="L20nCore.L20n.IO.Parsers.Comment"/>
				/// </summary>
				public sealed class Comment : INode
				{
					public Comment(string value)
					{
						m_Value = value;
					}
					
					public L20n.Objects.L20nObject Eval()
					{
						return L20n.Objects.L20nObject.Nop;
					}
					
					public string Display()
					{
						return "#" + m_Value;
					}
					
					private readonly string m_Value;
				}
			}
		}
	}
}
