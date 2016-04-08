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

namespace L20nCore
{
	namespace IO
	{
		namespace AST
		{
			/// <summary>
			/// The AST representation for an exernal object, given by the user.
			/// More Information: <see cref="L20nCore.IO.Parsers.Variable"/> 
			/// </summary>
			public sealed class Variable : INode
			{
				public string Value
				{
					get { return m_Value; }
				}
				
				public Variable(string value)
				{
					m_Value = value;
				}
				
				public Objects.L20nObject Eval()
				{
					return new Objects.Variable(m_Value).Optimize();
				}
				
				public string Display()
				{
					return String.Format("${0}", m_Value);
				}
				
				private readonly string m_Value;
			}
		}
	}
}
