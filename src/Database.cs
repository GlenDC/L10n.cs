using System;
using System.Collections.Generic;
using System.IO;

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

		public Locale CurrentLocale
		{
			get { return m_CurrentLocale; }
		}

		private Locale m_CurrentLocale;
		private Manifest m_Manifest;

		public Database()
		{
			m_CurrentLocale = null;
			m_Manifest = new Manifest();
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
					throw new IOException(msg);
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
					throw new IOException(msg);
				}
				
				var resources = root["resources"];
				if(resources == null || resources.Count == 0)
				{
					string msg = string.Format("No resources were provided in: {0}", manifest_path);
					throw new IOException(msg);
				}
				foreach(var resource in resources.Children)
				{
					m_Manifest.AddResoure(resource.Value, manifest_path);
				}
			}
		}

		public void LoadLocale(string id = null)
		{
			if (id == null)
			{
				id = m_Manifest.DefaultLocale;
			}

			// get resources
			var localeFiles = m_Manifest.GetLocaleFiles(id);
			if (localeFiles.Count == 0)
			{
				string msg = string.Format("No resources were found for locale: {0}", id);
				throw new IOException(msg);
			}

			m_CurrentLocale = new Locale();
			foreach(var name in localeFiles)
			{
				m_CurrentLocale.Import(name);
			}
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
						throw new IOException(msg);
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
					throw new IOException(msg);
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

