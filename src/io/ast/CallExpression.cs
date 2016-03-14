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

namespace L20n
{
	namespace IO
	{
		namespace AST
		{
			public sealed class CallExpression : INode
			{
				private readonly INode m_Name;
				private List<INode> m_Parameters;
				
				public CallExpression(INode name, INode parameter)
				{
					m_Name = name;
					m_Parameters = new List<INode>(1);
					AddParameter(parameter);
				}
				
				public void AddParameter(INode parameter)
				{
					m_Parameters.Add(parameter);
				}
				
				public Objects.L20nObject Eval()
				{
					var name = ((Objects.Identifier)m_Name.Eval()).Value;
					var parameters = new Objects.L20nObject[m_Parameters.Count];
					for (int i = 0; i < parameters.Length; ++i)
						parameters [i] = m_Parameters [i].Eval ();
					return new Objects.CallExpression(name, parameters);
				}
				
				public string Display()
				{
					var parameters = new string[m_Parameters.Count];
					for (int i = 0; i < parameters.Length; ++i)
						parameters[i] = m_Parameters[i].Display ();

					return String.Format("{0}({1})",
						m_Name.Display(), String.Join(",", parameters));
				}
			}
		}
	}
}
