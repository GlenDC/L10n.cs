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
			public sealed class Macro : INode
			{
				private readonly string m_Identifier;
				private List<string> m_Parameters;
				private INode m_Expression;
				
				public Macro(string identifier)
				{
					m_Identifier = identifier;
					m_Parameters = new List<string>();
					m_Expression = null;
				}

				public void SetExpression(INode expression)
				{
					if(m_Expression != null)
						throw new ParseException("can't set expression as it's already set");

					m_Expression = expression;
				}

				public void AddParameter(string variable)
				{
					m_Parameters.Add(variable);
				}
				
				public Objects.L20nObject Eval()
				{
					var expression = m_Expression.Eval();

					return new Objects.Macro(
						m_Identifier, expression, m_Parameters.ToArray()).Optimize();
				}
				
				public string Display()
				{;
					return String.Format("{0}({1}){{2}}",
						m_Identifier,
					    String.Join(",", m_Parameters),
					    m_Expression.Display());
				}
			}
		}
	}
}
