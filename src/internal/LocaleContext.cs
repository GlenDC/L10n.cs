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

using L20nCore.Utils;
using L20nCore.Objects;
using L20nCore.Exceptions;

namespace L20nCore
{
	namespace Internal
	{
		public sealed class LocaleContext
		{	
			private readonly Utils.DictionaryRef<string, L20nObject> m_Globals;
			private readonly Dictionary<string, Macro> m_Macros;
			private readonly Dictionary<string, Entity> m_Entities;
			private readonly ShadowStack<L20nObject> m_Variables;
			private readonly LocaleContext m_Parent;

			public LocaleContext(
				Utils.DictionaryRef<string, L20nObject> globals,
				Dictionary<string, Macro> macros,
				Dictionary<string, Entity> entities,
				LocaleContext parent)
			{
				m_Globals = globals;
				m_Macros = new Dictionary<string, Macro>(macros);
				m_Entities = new Dictionary<string, Entity>(entities);
				m_Variables = new ShadowStack<L20nObject>();
				m_Parent = parent;
			}
			
			public L20nObject GetGlobal(string key)
			{
				var global = GetGlobalPrivate(key);
				if (global != null)
					return global;

				if (m_Parent != null)
					global = m_Parent.GetGlobalPrivate(key);

				return global;
			}
			
			public Macro GetMacro(string key)
			{
				var macro = GetMacroPrivate(key);
				if (macro != null)
					return macro;
				
				if (m_Parent != null)
					macro = m_Parent.GetMacroPrivate(key);
				
				return macro;
			}
			
			public Entity GetEntity(string key)
			{
				var entity = GetEntityPrivate(key);
				if (entity != null)
					return entity;
				
				if (m_Parent != null)
					entity = m_Parent.GetEntityPrivate(key);
				
				return entity;
			}
			
			private L20nObject GetGlobalPrivate(string key)
			{
				return m_Globals.Get(key, null);
			}
			
			private Macro GetMacroPrivate(string key)
			{
				Macro macro;
				if (m_Macros.TryGetValue(key, out macro))
				{
					return macro;
				}
				
				return null;
			}
			
			private Entity GetEntityPrivate(string key)
			{
				Entity entity;
				if (m_Entities.TryGetValue(key, out entity))
				{
					return entity;
				}
				
				return null;
			}
			
			public void PushVariable(string key, L20nObject value)
			{
				m_Variables.Push(key, value);
			}
			
			public void DropVariable(string key)
			{
				if (m_Variables.PopSafe(key) == null)
				{
					Internal.Logger.WarningFormat(
						"couldn't drop variable with key {0}", key);
				}
			}
			
			public L20nObject GetVariable(string key)
			{
				return m_Variables.PeekSafe(key);
			}

			public class Builder
			{
				private readonly Dictionary<string, Macro> m_Macros;
				private readonly Dictionary<string, Entity> m_Entities;
				
				public Builder()
				{
					m_Macros = new Dictionary<string, Macro>();
					m_Entities = new Dictionary<string, Entity>();
				}

				public void Import(String file_name)
				{
					try
					{
						IO.LocalizbleObjectsList.Parse(file_name, this);
					} catch (Exception e)
					{
						string msg = String.Format(
							"something went wrong importing locale file: {0}",
							file_name);
						throw new ImportException(msg, e);
					}
				}
				
				public void AddMacro(string key, Macro obj)
				{
					try
					{
						m_Macros.Add(key, obj);
					} catch (ArgumentException)
					{
						throw new Exceptions.ImportException(
							String.Format(
								"macro with key {0} can't be added, as key isn't unique",
								key));
					}
				}
				
				public void AddEntity(string key, Entity obj)
				{
					try
					{
						m_Entities.Add(key, obj);
					} catch (ArgumentException)
					{
						throw new Exceptions.ImportException(
							String.Format("entity with key {0} can't be added, as key isn't unique", key));
					}
				}
				
				public LocaleContext Build(Dictionary<string, L20nObject> globals, LocaleContext parent)
				{
					var globalsRef = new Utils.DictionaryRef<string, L20nObject>(globals);
					return new LocaleContext(globalsRef, m_Macros, m_Entities, parent);
				}
			}
		}
	}
}
