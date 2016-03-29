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
		/// <summary>
		/// The LocaleContext is used by the L20n Objects to
		/// get access to Macros, Entities and where they can push, use, and pop temporary variables.
		/// </summary>
		public sealed class LocaleContext
		{	
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Internal.LocaleContext"/> class.
			/// </summary>
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

			/// <summary>
			/// Get a <see cref="L20nCore.Objects.Global"/> with the <c>key</c> found
			/// either in the <c>CurrentLocale</c> or <c>DefaultLocale</c>.
			/// <c>null</c> gets returned in case no matching <see cref="L20nCore.Objects.Global"/> could be found.
			/// </summary>
			public L20nObject GetGlobal(string key)
			{
				var global = GetGlobalPrivate(key);
				if (global != null)
					return global;

				if (m_Parent != null)
					global = m_Parent.GetGlobalPrivate(key);

				return global;
			}

			/// <summary>
			/// Get a <see cref="L20nCore.Objects.Macro"/> with the <c>key</c> found
			/// either in the <c>CurrentLocale</c> or <c>DefaultLocale</c>.
			/// <c>null</c> gets returned in case no matching <see cref="L20nCore.Objects.Macro"/> could be found.
			/// </summary>
			public Macro GetMacro(string key)
			{
				var macro = GetMacroPrivate(key);
				if (macro != null)
					return macro;
				
				if (m_Parent != null)
					macro = m_Parent.GetMacroPrivate(key);
				
				return macro;
			}

			/// <summary>
			/// Get a <see cref="L20nCore.Objects.Entity"/> with the <c>key</c> found
			/// either in the <c>CurrentLocale</c> or <c>DefaultLocale</c>.
			/// <c>null</c> gets returned in case no matching <see cref="L20nCore.Objects.Entity"/> could be found.
			/// </summary>
			public Entity GetEntity(string key)
			{
				var entity = GetEntityPrivate(key);
				if (entity != null)
					return entity;
				
				if (m_Parent != null)
					entity = m_Parent.GetEntityPrivate(key);
				
				return entity;
			}

			/// <summary>
			/// Pushes <c>value</c> on the stack with
			/// the value of <c>key</c> as its name.
			/// </summary>
			public void PushVariable(string key, L20nObject value)
			{
				m_Variables.Push(key, value);
			}

			/// <summary>
			/// Drops the last variable linked with <c>key</c> from the stack.
			/// A warning gets logged in case no variable was linked with the given <c>key</c>.
			/// </summary>
			public void DropVariable(string key)
			{
				if (m_Variables.PopSafe(key) == null)
				{
					Internal.Logger.WarningFormat(
						"couldn't drop variable with key {0}", key);
				}
			}

			/// <summary>
			/// Get the last variable pushed on the stack and matched with <c>key</c>.
			/// <c>null</c> gets returned in case no such variable could be found.
			/// </summary>
			public L20nObject GetVariable(string key)
			{
				return m_Variables.PeekSafe(key);
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

			private readonly Utils.DictionaryRef<string, L20nObject> m_Globals;
			private readonly Dictionary<string, Macro> m_Macros;
			private readonly Dictionary<string, Entity> m_Entities;
			private readonly ShadowStack<L20nObject> m_Variables;
			private readonly LocaleContext m_Parent;

			/// <summary>
			/// A class used to create an instance of <see cref="L20nCore.Internal.LocaleContext"/>.
			/// </summary>
			public class Builder
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.Internal.LocaleContext+Builder"/> class.
				/// </summary>
				public Builder()
				{
					m_Macros = new Dictionary<string, Macro>();
					m_Entities = new Dictionary<string, Entity>();
				}

				/// <summary>
				/// Import a Localization (L20n Language) File.
				/// Meaning that its content will get parsed and turned into L20n Objects.
				/// </summary>
				public void Import(String file_name)
				{
					try
					{
						IO.LocalizableObjectsList.ImportAndParse(file_name, this);
					} catch (Exception e)
					{
						string msg = String.Format(
							"something went wrong importing locale file: {0}",
							file_name);
						throw new ImportException(msg, e);
					}
				}

				/// <summary>
				/// Add a macro with <c>key</c> as its name and <c>obj</c> as its value.
				/// Throws an <see cref="L20nCore.Exceptions.ImportException"/> in case
				/// a macro is already added with <c>key</c> as its name.
				/// </summary>
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

				/// <summary>
				/// Add an entity with <c>key</c> as its name and <c>obj</c> as its value.
				/// Throws an <see cref="L20nCore.Exceptions.ImportException"/> in case
				/// an entity is already added with <c>key</c> as its name.
				/// </summary>
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

				/// <summary>
				/// Create an instance of <see cref="L20nCore.Internal.LocaleContext"/>
				/// using the added objects and imported localization files.
				/// </summary>
				public LocaleContext Build(Dictionary<string, L20nObject> globals, LocaleContext parent)
				{
					var globalsRef = new Utils.DictionaryRef<string, L20nObject>(globals);
					return new LocaleContext(globalsRef, m_Macros, m_Entities, parent);
				}

				private readonly Dictionary<string, Macro> m_Macros;
				private readonly Dictionary<string, Entity> m_Entities;
			}
		}
	}
}
