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

using L20n.Internal;

using SimpleJSON;

namespace L20n
{
	public class Database
	{
		public string DefaultLocale
		{
			get { return m_Manifest.DefaultLocale; }
		}

		public List<string> Locales
		{
			get { return m_Manifest.Locales; }
		}
		
		private Internal.Locale m_DefaultLocale;
		private Internal.Locale m_CurrentLocale;
		private Manifest m_Manifest;

		private Dictionary<string, L20n.Objects.GlobalValue> m_Globals;

		public Database()
		{
			m_CurrentLocale = null;
			m_Manifest = new Manifest();
			m_Globals = new Dictionary<string, L20n.Objects.GlobalValue>();

			AddSystemGlobals();
		}

		public void AddGlobal(string id, L20n.Objects.GlobalLiteral.Delegate callback)
		{
			AddGlobalValue(id, new L20n.Objects.GlobalLiteral(callback));
		}
		
		public void AddGlobal(string id, L20n.Objects.GlobalString.Delegate callback)
		{
			AddGlobalValue(id, new L20n.Objects.GlobalString(callback));
		}

		public void Import(string manifest_path)
		{
			using(var sr = new StreamReader(manifest_path))
			{
				var json = sr.ReadToEnd();
				var root = JSON.Parse(json);

				var locales = root["locales"];
				if(locales == null || locales.Count == 0)
				{
					string msg = string.Format("No locales were provided in: {0}", manifest_path);
					throw new L20n.Exceptions.ImportException(msg);
				}
				foreach(var locale in locales.Children)
				{
					m_Manifest.AddLocale(locale.Value);
				}

				var defaultLocale = root["default_locale"].Value;
				if(defaultLocale != "")
				{
					m_Manifest.DefaultLocale = defaultLocale;
				}
				if(m_Manifest.DefaultLocale == null)
				{
					string msg = string.Format("No default locale was provided in: {0}", manifest_path);
					throw new L20n.Exceptions.ImportException(msg);
				}

				var resources = root["resources"];
				if(resources == null || resources.Count == 0)
				{
					string msg = string.Format("No resources were provided in: {0}", manifest_path);
					throw new L20n.Exceptions.ImportException(msg);
				}
				foreach(var resource in resources.Children)
				{
					m_Manifest.AddResoure(resource.Value, manifest_path);
				}

				// lastly, already import the default, as we'll always need that one
				ImportLocal(m_Manifest.DefaultLocale, out m_DefaultLocale);
			}
		}

		public void LoadLocale(string id)
		{
			if (id == null) {
				throw new L20n.Exceptions.ImportException(
					"a locale id has to be given in order to load one");
			}

			if (id == DefaultLocale)
				m_CurrentLocale = null;
			else
				ImportLocal (id, out m_CurrentLocale);
		}

		public string Translate(string id)
		{
			var output = m_DefaultLocale.Context.GetEntity(id).Eval(m_DefaultLocale.Context);
			return output.As<L20n.Objects.StringOutput>().Value;

			/*try {
				try {
					var output = m_CurrentLocale.Context.GetEntity(id).Eval(m_CurrentLocale.Context);
					return output.As<L20n.Objects.StringOutput>().Value;
				}
				catch(Exception e) {
					Console.WriteLine(e);
					var output = m_DefaultLocale.Context.GetEntity(id).Eval(m_CurrentLocale.Context);
					return output.As<L20n.Objects.StringOutput>().Value;
				}
			}
			catch(Exception e) {
				Console.WriteLine(e);
				// ignore exception for now
				return id;
			}*/
		}

		private void ImportLocal(string id, out Locale locale)
		{
			var localeFiles = m_Manifest.GetLocaleFiles(id);
			if (localeFiles.Count == 0)
			{
				string msg = string.Format("No resources were found for locale: {0}", id);
				throw new L20n.Exceptions.ImportException(msg);
			}
			
			var builder = new LocaleBuilder();
			foreach(var name in localeFiles)
			{
				builder.Import(name);
			}

			locale = builder.BuildLocale(m_Globals);
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

		private class Manifest
		{
			private const string LOCALE_STRING_ID = "{{locale}}";

			private List<string> m_Locales;
			private string m_DefaultLocale;
			private List<string> m_Resources;

			public List<string> Locales
			{
				get { return m_Locales; }
			}

			public string DefaultLocale
			{
				get { return m_DefaultLocale; }
				set
				{
					if(!m_Locales.Contains(value))
					{
						string msg = string.Format("{0} is not a valid default locale as " +
						                           "it couldn't be found in the list of locales.", value);
						throw new L20n.Exceptions.ImportException(msg);
					}

					m_DefaultLocale = value;
				}
			}

			public Manifest()
			{
				m_Locales = new List<string>();
				m_DefaultLocale = null;
				m_Resources = new List<string>();
			}

			public void AddLocale(string locale)
			{
				m_Locales.Add(locale);
			}

			public void AddResoure(string resource, string manifest)
			{
				if (!resource.Contains(LOCALE_STRING_ID))
				{
					string msg = string.Format("Resource '{0}' does not contain local string-id ({1}).",
					                           resource, LOCALE_STRING_ID);
					throw new L20n.Exceptions.ImportException(msg);
				}

				resource = Path.Combine(new string[] {
				    Path.GetDirectoryName(manifest),
					resource});

				m_Resources.Add(resource);
			}

			public List<String> GetLocaleFiles(string locale)
			{
				var files = new List<String>();
				foreach(var resource in m_Resources) {
					var file = resource.Replace(LOCALE_STRING_ID, locale);
					if(File.Exists(file))
					{
						files.Add(file);
					}
				}

				return files;
			}
		}
	}
}
