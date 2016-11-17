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
				/// The AST representation for an Entity value.
				/// More Information: <see cref="L20nCore.IO.Parsers.Entity"/>
				/// </summary>
				public sealed class Entity : INode
				{
					public Entity(string identifier, bool is_private, INode index, INode value, INode attributes)
					{
						m_Identifier = identifier;
						m_Index = index;
						m_Value = value;
						m_Attributes = attributes;
						m_IsPrivate = is_private;
					}
				
					public Objects.L20nObject Eval()
					{
						Objects.L20nObject index =
						m_Index != null ? m_Index.Eval() : null;
						var value = m_Value.Eval();
						Objects.L20nObject attributes =
						m_Attributes != null ? m_Attributes.Eval() : null;

						return new Objects.Entity(index, m_IsPrivate, value, attributes).Optimize();
					}
				
					public string Display()
					{
						return String.Format("<{0}{1} {2}{3}>",
						m_Identifier,
					    m_Index != null ? ((Index)m_Index).Display() : "",
					    m_Value.Display(),
					    m_Attributes != null ? m_Attributes.Display() : "");
					}

					private readonly string m_Identifier;
					private readonly INode m_Index;
					private readonly INode m_Value;
					private readonly INode m_Attributes;
					private readonly bool m_IsPrivate;
				}
			}
		}
	}
}
