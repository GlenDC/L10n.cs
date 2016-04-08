/**
 * This source file is part of the Commercial L20n Unity Plugin.
 *
 * Copyright (c) 2016 Glen De Cauwsemaecker (contact@glendc.com)
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

using L20nCore.Exceptions;

namespace L20nCore
{
	namespace IO
	{
		namespace AST
		{
			/// <summary>
			/// The AST representation for an Entity value.
			/// More Information: <see cref="L20nCore.IO.Parsers.Entity"/>
			/// </summary>
			public sealed class Entity : INode
			{
				public Entity(string identifier, bool is_private, INode index, INode value)
				{
					m_Identifier = identifier;
					m_Index = index;
					m_Value = value;
					m_IsPrivate = is_private;
				}
				
				public Objects.L20nObject Eval()
				{
					Objects.L20nObject index =
						m_Index != null ? m_Index.Eval() : null;
					var value = m_Value.Eval();

					return new Objects.Entity(index, m_IsPrivate, value).Optimize();
				}
				
				public string Display()
				{
					return String.Format("<{0}{1} {2}>",
						m_Identifier,
					    m_Index != null ? ((Index)m_Index).Display() : "",
					    m_Value.Display());
				}

				private readonly string m_Identifier;
				private readonly INode m_Index;
				private readonly INode m_Value;
				private readonly bool m_IsPrivate;
			}
		}
	}
}
