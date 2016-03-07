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
using System.IO;
using L20n.Objects;

namespace L20n
{
	namespace Internal
	{
		public sealed class Context
		{
			private readonly Dictionary<string, GlobalValue> m_Globals;
			private readonly Dictionary<string, Macro> m_Macros;
			private readonly Dictionary<string, Entity> m_Entities;

			public Context(
				Dictionary<string, GlobalValue> globals,
				Dictionary<string, Macro> macros,
				Dictionary<string, Entity> entities)
			{
				m_Globals = globals;
				m_Macros = macros;
				m_Entities = entities;
			}

			public GlobalValue GetGlobal(string key)
			{
				try {
					return m_Globals[key];
				}
				catch(KeyNotFoundException exception) {
					throw new L20n.Exceptions.ObjectNotFoundException(
						String.Format("global `{0}` doesn't exist", key));
				}
			}
			
			public Macro GetMacro(string key)
			{
				try {
					return m_Macros[key];
				}
				catch(KeyNotFoundException exception) {
					throw new L20n.Exceptions.ObjectNotFoundException(
						String.Format("macro `{0}` doesn't exist", key));
				}
			}
			
			public Entity GetEntity(string key)
			{
				try {
					return m_Entities[key];
				}
				catch(KeyNotFoundException exception) {
					throw new L20n.Exceptions.ObjectNotFoundException(
						String.Format("entity `{0}` doesn't exist", key));
				}
			}

			public Variable GetVariable(string key)
			{
				throw new L20n.Exceptions.ObjectNotFoundException(
					String.Format("getting variables is not supported yet", key));
			}
		}
	}
}
