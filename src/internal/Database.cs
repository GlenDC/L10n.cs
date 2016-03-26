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

using L20nCore.Exceptions;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Internal
	{
		public sealed class Database
		{
			public bool IsInitialized
			{
				get { return CurrentLocale != null; }
			}

			public Manifest Manifest { get; private set; }
			public string CurrentLocale { get; private set; }

			private LocaleContext m_DefaultContext;
			private LocaleContext m_CurrentContext;

			private Objects.Dummy m_DummyObject;

			private readonly Dictionary<string, Objects.L20nObject> m_Globals;

			public Database()
			{
				Manifest = new Manifest();
				m_Globals = new Dictionary<string, Objects.L20nObject>();

				m_DummyObject = new Objects.Dummy();

				CurrentLocale = null;

				m_DefaultContext = null;
				m_CurrentContext = null;

				AddSystemGlobals();
			}

			public void Import(string manifest_path)
			{
				m_DefaultContext = null;
				m_CurrentContext = null;

				Manifest.Import(manifest_path);
				ImportLocal(Manifest.DefaultLocale, out m_DefaultContext, null);

				CurrentLocale = Manifest.DefaultLocale;
			}

			public void LoadLocale(string id)
			{
				if (id == null) {
					throw new ImportException(
						"a locale id has to be given in order to load one");
				}

				if (id == Manifest.DefaultLocale) {
					m_CurrentContext = null;
					CurrentLocale = Manifest.DefaultLocale;
				} else if (IsInitialized) {
					ImportLocal (id, out m_CurrentContext, m_DefaultContext);
					CurrentLocale = id;
				} else {
					throw new ImportException(
						"couldn't load locale as the L20n databse has no been initialized");
				}
			}

			public string Translate(string id)
			{
				return TranslateID(id);
			}

			public string Translate(string id, string[] keys, Objects.L20nObject[] values)
			{
				if (keys.Length == 0) {
					Internal.Logger.WarningFormat(
						"couldn't translate {0} as no keys were given", id);
					return id;
				}

				if (keys.Length != values.Length) {
					Internal.Logger.WarningFormat(
						"couldn't translate {0} as the amount of keys and values isn't equal," +
						" expected {1}, got {2}", id, keys.Length, values.Length);
					return id;
				}

				var ctx = m_CurrentContext != null ? m_CurrentContext : m_DefaultContext;
				if (ctx == null) {
					Logger.WarningFormat("couldn't translate {0} as no context has been set", id);
					return id;
				}

				// push all variables to the stack
				for(int i = 0; i < keys.Length; ++i) {
					if(keys[i] == null) {
						Logger.WarningFormat("couldn't translate {0} because parameter-key #{1} is null" +
						                     " while expecting an string", id, i);
						break;
					}

					if(values[i] == null) {
						Logger.WarningFormat("couldn't translate {0} because parameter-value #{1} is null",
						                     id, i);
						break;
					}

					ctx.PushVariable(keys[i], values[i]);
				}

				var output = TranslateID(id);

				// remove variables from stack again
				for(int i = 0; i < keys.Length; ++i) {
					ctx.DropVariable(keys[i]);
				}

				return output;
			}
			
			public void AddGlobal(string id, int value)
			{
				AddGlobal(id, new Objects.Literal(value));
			}
			
			public void AddGlobal(string id, string value)
			{
				AddGlobal(id, new Objects.StringOutput(value));
			}
			
			public void AddGlobal(string id, External.IHashValue value)
			{
				AddGlobal(id, new Objects.Entity(value));
			}
			
			public void AddGlobal(string id, Objects.DelegatedLiteral.Delegate callback)
			{
				AddGlobal(id, new Objects.DelegatedLiteral(callback));
			}
			
			public void AddGlobal(string id, Objects.DelegatedString.Delegate callback)
			{
				AddGlobal(id, new Objects.DelegatedString(callback));
			}

			public void AddGlobal(string id, Objects.L20nObject value)
			{
				if(value == null) {
					Logger.WarningFormat(
						"global user-variable {0} couldn't be unwrapped", id);
					return;
				}

				try {
					m_Globals.Add(id, value);
				}
				catch(ArgumentException) {
					Logger.WarningFormat(
						"global value with id {0} isn't unique, " +
						"and old value will be overriden", id);
					m_Globals[id] = value;
				}
			}

			private void ImportLocal(string id, out LocaleContext context, LocaleContext parent)
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

				context = builder.Build(m_Globals, parent);
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

				var context = m_CurrentContext != null ? m_CurrentContext : m_DefaultContext;
				if (context == null) {
					Logger.WarningFormat(
						"{0} could not be translated as no language-context has been set", id);
					return id;
				}

				var output = identifier.Eval(context, m_DummyObject)
					as Objects.StringOutput;

				if (output == null) {
					Internal.Logger.WarningFormat(
						"something went wrong, {0} could not be translated", id);
					return id;
				}

				return output.Value;
			}
		}
	}
}
