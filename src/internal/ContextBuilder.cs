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

using L20n.Objects;

namespace L20n
{
	namespace Internal
	{
		public sealed class ContextBuilder
		{
			private Dictionary<string, Macro> m_Macros;
			private Dictionary<string, Entity> m_Entities;
			
			public ContextBuilder()
			{
				m_Macros = new Dictionary<string, Macro>();
				m_Entities = new Dictionary<string, Entity>();
			}
			
			public void AddMacro(string key, Macro obj)
			{
				try {
					m_Macros.Add(key, obj);
				}
				catch(ArgumentException) {
					throw new L20n.Exceptions.ImportException(
						String.Format("macro with key {0} can't be added, as key isn't unique", key));
				}
			}
			
			public void AddEntity(string key, Entity obj)
			{
				try {
					m_Entities.Add(key, obj);
				}
				catch(ArgumentException) {
					throw new L20n.Exceptions.ImportException(
						String.Format("entity with key {0} can't be added, as key isn't unique", key));
				}
			}

			public Context BuildContext(Dictionary<string, Global> globals)
			{
				return new Context(globals, m_Macros, m_Entities);
			}
		}
	}
}
