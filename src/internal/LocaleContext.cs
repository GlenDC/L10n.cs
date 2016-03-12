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

using L20n.Utils;
using L20n.Objects;
using L20n.Exceptions;

namespace L20n
{
	namespace Internal
	{
		public sealed class LocaleContext
		{	
			private readonly Utils.DictionaryRef<string, GlobalValue> m_Globals;
			private readonly Dictionary<string, Macro> m_Macros;
			private readonly Dictionary<string, Entity> m_Entities;
			private readonly ShadowStack<L20nObject> m_Variables;

			public LocaleContext(
				Utils.DictionaryRef<string, GlobalValue> globals,
				Dictionary<string, Macro> macros,
				Dictionary<string, Entity> entities)
			{
				m_Globals = globals;
				m_Macros = macros;
				m_Entities = entities;
				m_Variables = new ShadowStack<L20nObject>();
			}
			
			public GlobalValue GetGlobal(string key)
			{
				try {
					return m_Globals.Get(key);
				}
				catch(KeyNotFoundException exception) {
					throw new Exceptions.ObjectNotFoundException(
						String.Format("global `{0}` doesn't exist", key),
						exception);
				}
			}
			
			public Macro GetMacro(string key)
			{
				try {
					return m_Macros[key];
				}
				catch(KeyNotFoundException exception) {
					throw new Exceptions.ObjectNotFoundException(
						String.Format("macro `{0}` doesn't exist", key),
						exception);
				}
			}
			
			public Entity GetEntity(string key)
			{
				try {
					return m_Entities[key];
				}
				catch(KeyNotFoundException exception) {
					throw new Exceptions.ObjectNotFoundException(
						String.Format("entity `{0}` doesn't exist", key),
						exception);
				}
			}
			
			public Option<Entity> GetEntitySafe(string key)
			{
				Entity entity;

				if (m_Entities.TryGetValue (key, out entity)) {
					return new Option<Entity>(entity);
				}

				return new Option<Entity>();
			}
			
			public void PushVariable(string key, L20nObject value)
			{
				m_Variables.Push(key, value);
			}
			
			public void DropVariable(string key)
			{
				m_Variables.Pop(key);
			}
			
			public L20nObject GetVariable(string key)
			{
				return m_Variables.Peek(key);
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
					try {
						IO.LocalizbleObjectsList.Parse(file_name, this);
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong importing locale file: {0}",
							file_name);
						throw new ImportException(msg, e);
					}
				}
				
				public void AddMacro(string key, Macro obj)
				{
					try {
						m_Macros.Add(key, obj);
					}
					catch(ArgumentException) {
						throw new Exceptions.ImportException(
							String.Format(
								"macro with key {0} can't be added, as key isn't unique",
								key));
					}
				}
				
				public void AddEntity(string key, Entity obj)
				{
					try {
						m_Entities.Add(key, obj);
					}
					catch(ArgumentException) {
						throw new Exceptions.ImportException(
							String.Format("entity with key {0} can't be added, as key isn't unique", key));
					}
				}
				
				public LocaleContext Build(Dictionary<string, GlobalValue> globals)
				{
					var globalsRef = new Utils.DictionaryRef<string, GlobalValue>(globals);
					return new LocaleContext(globalsRef, m_Macros, m_Entities);
				}
			}
		}
	}
}
