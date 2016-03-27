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

namespace L20nCore
{
	namespace IO
	{
		namespace AST
		{
			public sealed class PropertyExpression : INode
			{
				private List<INode> m_Identifiers;
				
				public PropertyExpression(INode root)
				{
					m_Identifiers = new List<INode>();
					m_Identifiers.Add(root);
				}

				public void Add(string id)
				{
					m_Identifiers.Add(new AST.Identifier(id));
				}
				
				public Objects.L20nObject Eval()
				{
					var identifiers = new Objects.L20nObject[m_Identifiers.Count];
					for (int i = 0; i < identifiers.Length; ++i)
						identifiers [i] = m_Identifiers [i].Eval();
					return new Objects.PropertyExpression(identifiers).Optimize();
				}
				
				public string Display()
				{
					var identifiers = new string[m_Identifiers.Count];
					for (int i = 0; i < identifiers.Length; ++i)
						identifiers [i] = m_Identifiers [i].Display();
					return String.Join(".", identifiers);
				}
			}
		}
	}
}
