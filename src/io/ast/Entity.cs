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

using L20n.Exceptions;

namespace L20n
{
	namespace IO
	{
		namespace AST
		{
			public sealed class Entity : INode
			{
				private readonly string m_Identifier;
				private readonly Utils.Optional<INode> m_Index;
				private readonly INode m_Value;
				
				public Entity(string identifier, INode index, INode value)
				{
					m_Identifier = identifier;
					m_Index = new Utils.Optional<INode>(index);
					m_Value = value;
				}
				
				public L20n.Objects.L20nObject Eval()
				{
					Utils.Optional<L20n.Objects.L20nObject> index;
					if (m_Index.IsSet)
						index = new Utils.Optional<L20n.Objects.L20nObject>(
							m_Index.Unwrap<INode>().Eval());
					else
						index = new Utils.Optional<L20n.Objects.L20nObject>();
					var value = m_Value.Eval();

					return new L20n.Objects.Entity(index, value);
				}
				
				public string Display()
				{
					return String.Format("<{0}{1} {2}>",
						m_Identifier,
					    m_Index.IsSet ? m_Index.Unwrap<Index>().Display() : "",
					    m_Value.Display());
				}
			}
		}
	}
}
