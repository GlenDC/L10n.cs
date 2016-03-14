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

			private Cache m_Cache;
			
			private readonly Dictionary<string, Objects.GlobalValue> m_Globals;

			public Database()
			{
				Manifest = new Manifest();
				m_Globals = new Dictionary<string, Objects.GlobalValue>();

				m_DefaultContext = new Option<LocaleContext>();
				m_CurrentContext = new Option<LocaleContext>();

				m_Cache = new Cache();

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
			
			public string Translate(string id, params External.IVariable[] variables)
			{
				if(variables == null || variables.Length == 0)
					return TranslateSimple(id);
				return TranslateWithVariables(id, variables);
			}
			
			public void AddGlobal(string id, L20n.Objects.GlobalLiteral.Delegate callback)
			{
				AddGlobalValue(id, new L20n.Objects.GlobalLiteral(callback));
			}
			
			public void AddGlobal(string id, L20n.Objects.GlobalString.Delegate callback)
			{
				AddGlobalValue(id, new L20n.Objects.GlobalString(callback));
			}
			
			private void ImportLocal(string id, Option<LocaleContext> context, LocaleContext parent)
			{
				m_Cache.Clear();

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

			private string TranslateSimple(string id)
			{
				return m_Cache.TryGet(id).OrElse(() => {
					string value = TranslateID(id);
					m_Cache.Set(id, value);
					return new Option<string>(value);
				}).UnwrapOr(id);
			}

			private string TranslateWithVariables(string id, params External.IVariable[] variables)
			{
				return m_CurrentContext.Or(m_DefaultContext).MapOr(id, (ctx) => {
					var info = new External.InfoCollector();
					var identifiers = new string[variables.Length];

					// push all variables to the stack
					for(int i = 0; i < identifiers.Length; ++i) {
						variables[i].Collect(out identifiers[i], info);
						ctx.PushVariable(identifiers[i], info.Collect());
						info.Clear();
					}

					var output = TranslateID(id);

					// remove variables from stack again
					for(int i = 0; i < identifiers.Length; ++i) {
						ctx.DropVariable(identifiers[i]);
					}

					return output;
				});
			}
		}
	}
}
