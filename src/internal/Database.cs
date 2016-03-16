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
			
			private Option<LocaleContext> m_DefaultContext;
			private Option<LocaleContext> m_CurrentContext;
			
			private readonly Dictionary<string, Objects.L20nObject> m_Globals;

			public Database()
			{
				Manifest = new Manifest();
				m_Globals = new Dictionary<string, Objects.L20nObject>();

				m_DefaultContext = new Option<LocaleContext>();
				m_CurrentContext = new Option<LocaleContext>();

				AddSystemGlobals();
			}

			public void Import(string manifest_path)
			{
				m_DefaultContext = new Option<LocaleContext>();
				m_CurrentContext = new Option<LocaleContext>();

				Manifest.Import(manifest_path);
				ImportLocal(Manifest.DefaultLocale, m_DefaultContext, null);

				CurrentLocale = Manifest.DefaultLocale;
			}

			public void LoadLocale(string id)
			{
				if (id == null) {
					throw new ImportException(
						"a locale id has to be given in order to load one");
				}
				
				if (id == Manifest.DefaultLocale) {
					m_CurrentContext.Unset ();
					CurrentLocale = Manifest.DefaultLocale;
				} else {
					ImportLocal (id, m_CurrentContext, m_DefaultContext.Unwrap ());
					CurrentLocale = id;
				}
			}
			
			public string Translate(string id)
			{
				return TranslateID(id);
			}

			public string Translate(string id, UserVariables variables)
			{
				if(variables.Count == 0)
					return TranslateID(id);

				return m_CurrentContext.Or(m_DefaultContext).MapOr(id, (ctx) => {
					// push all variables to the stack
					int i = 0;
					foreach(var variable in variables) {
						if(variable.Key == null) {
							Logger.WarningFormat("couldn't translate {0} because parameter-key #{1} is null" +
							                     " while expecting an string", id, i);
							break;
						}

						if(variable.Value == null || !variable.Value.Value.IsSet) {
							Logger.WarningFormat("couldn't translate {0} because parameter-value #{1} is null",
							                     id, i);
							break;
						}

						ctx.PushVariable(variable.Key, variable.Value.Value.Unwrap());
						++i;
					}
					
					var output = TranslateID(id);
					
					// remove variables from stack again
					foreach(var key in variables.Keys) {
						if(key != null)
							ctx.DropVariable(key);
					}
					
					return output;
				});
			}
			
			public void AddGlobal(string id, UserVariable value)
			{
				if (!value.Value.IsSet) {
					Logger.WarningFormat(
						"global user-variable {0} couldn't be unwrapped", id);
					return;
				}

				var global = value.Value.Unwrap();

				try {
					m_Globals.Add(id, global);
				}
				catch(ArgumentException) {
					Logger.WarningFormat(
						"global value with id {0} isn't unique, " +
						"and old value will be overriden", id);
					m_Globals[id] = global;
				}
			}
			
			public void AddGlobal(string id, Objects.DelegatedObject.Delegate callback)
			{
				AddGlobal(id, new UserVariable(callback));
			}
			
			private void ImportLocal(string id, Option<LocaleContext> context, LocaleContext parent)
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
				
				context.Set(builder.Build(m_Globals, parent));
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

			private string TranslateID(string id)
			{
				Objects.L20nObject identifier;
				
				if(id.IndexOf ('.') > 0)
					identifier = new Objects.PropertyExpression(id.Split('.'));
				else
					identifier = new Objects.IdentifierExpression(id);
				
				var context = m_CurrentContext.Or(m_DefaultContext);
				if (!context.IsSet) {
					Internal.Logger.WarningFormat(
						"{0} could not be translated as no language-context has been set", id);
					return id;
				}
				
				var output = identifier.Eval(
					context.Unwrap(), new Objects.Dummy())
					.UnwrapAs<Objects.StringOutput>();
				
				if (!output.IsSet) {
					Internal.Logger.WarningFormat(
						"something went wrong, {0} could not be translated", id);
					return id;
				}
				
				return output.Unwrap().Value;
			}
		}
	}
}
