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
using L20n.Utils;

namespace L20n
{
	namespace Internal
	{
		public sealed class Database
		{	
			public Manifest Manifest { get; private set; }
			public string CurrentLocale { get; private set; }
			
			private Optional<LocaleContext> m_DefaultContext;
			private Optional<LocaleContext> m_CurrentContext;
			
			private readonly Dictionary<string, Objects.GlobalValue> m_Globals;

			public Database()
			{
				Manifest = new Manifest();
				m_Globals = new Dictionary<string, Objects.GlobalValue>();

				m_DefaultContext = new Optional<LocaleContext>();
				m_CurrentContext = new Optional<LocaleContext>();

				AddSystemGlobals();
			}

			public void Import(string manifest_path)
			{
				Manifest.Import(manifest_path);
				ImportLocal(Manifest.DefaultLocale, m_DefaultContext);
			}

			public void LoadLocale(string id)
			{
				if (id == null) {
					throw new ImportException(
						"a locale id has to be given in order to load one");
				}
				
				if (id == Manifest.DefaultLocale)
					m_CurrentContext = null;
				else
					ImportLocal(id, m_CurrentContext);
			}
			
			public string Translate(string id)
			{
				Objects.Entity entity;
				LocaleContext ctx;

				if (m_CurrentContext.IsSet) {
					ctx = m_CurrentContext.Expect();
					if(ctx.GetEntity(id, out entity)) {
						var output = entity.Eval(ctx);
						return output.As<Objects.StringOutput>().Value;
					}
				}

				ctx = m_DefaultContext.Expect();
				if(ctx.GetEntity(id, out entity)) {
					var output = entity.Eval(ctx);
					return output.As<Objects.StringOutput>().Value;
				}

				return id;
			}
			
			public void AddGlobal(string id, L20n.Objects.GlobalLiteral.Delegate callback)
			{
				AddGlobalValue(id, new L20n.Objects.GlobalLiteral(callback));
			}
			
			public void AddGlobal(string id, L20n.Objects.GlobalString.Delegate callback)
			{
				AddGlobalValue(id, new L20n.Objects.GlobalString(callback));
			}
			
			private void ImportLocal(string id, Optional<LocaleContext> context)
			{
				var localeFiles = Manifest.GetLocaleFiles(id);
				if (localeFiles.Count == 0)
				{
					string msg = string.Format("No resources were found for locale: {0}", id);
					throw new Exceptions.ImportException(msg);
				}
				
				var builder = new LocaleContext.Builder();
				for(var i = 0; i < localeFiles.Count; ++i)
				{
					builder.Import(localeFiles[i]);
				}
				
				context.Set(builder.Build (m_Globals));
			}
			
			private void AddGlobalValue(string id, L20n.Objects.GlobalValue value)
			{
				try {
					m_Globals.Add(id, value);
				}
				catch(ArgumentException) {
					throw new L20n.Exceptions.ImportException(
						String.Format("global value with id {0} can't be added, as id isn't unique", id));
				}
			}
			
			private void AddSystemGlobals()
			{
				// time related
				AddGlobal("hour", () => System.DateTime.Now.Hour);
				AddGlobal("minute", () => System.DateTime.Now.Minute);
				AddGlobal("second", () => System.DateTime.Now.Second);
				
				// date related
				AddGlobal("year", () => System.DateTime.Today.Year);
				AddGlobal("month", () => System.DateTime.Today.Month);
				AddGlobal("day", () => System.DateTime.Today.Day);
			}
		}
	}
}
