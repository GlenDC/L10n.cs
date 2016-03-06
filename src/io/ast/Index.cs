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
			public sealed class Index : INode
			{
				private List<INode> m_Indeces;
				
				public Index(INode index)
				{
					m_Indeces = new List<INode>(1);
					AddIndex(index);
				}
				
				public void AddIndex(INode index)
				{
					m_Indeces.Add(index);
				}
				
				public L20n.Objects.L20nObject Eval()
				{
					var indeces = new L20n.Objects.L20nObject[m_Indeces.Count];
					for (int i = 0; i < indeces.Length; ++i)
						indeces[i] = m_Indeces[i].Eval();

					// TODO: IF all indeces are STATIC
						// create identifier if only one
						// ... create property if more
					
					return new L20n.Objects.Index(indeces);
				}
				
				public string Display()
				{
					if(m_Indeces.Count == 0)
						return "";

					var indeces = new string[m_Indeces.Count];
					for(int i = 0; i < indeces.Length; ++i)
						indeces[i] = m_Indeces[i].Display();
					return String.Format("[{0}]",
						String.Join (",", indeces));
				}
			}
		}
	}
}