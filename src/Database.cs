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

				foreach(var locale in root["locales"].Children)
				{
					m_Manifest.AddLocale(locale.Value);
				}

				m_Manifest.DefaultLocale = root["default_locale"].Value;

				foreach(var resource in root["resources"].Children)
				{
					m_Manifest.AddResoure(resource.Value);
				}
			}
		}

		public void LoadLocale(string id)
		{
			if (id == null)
			{
				id = m_Manifest.DefaultLocale;
			}

			// get resources

			m_CurrentLocale = new Locale();
		}

		private class Manifest
		{
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
				set { m_DefaultLocale = value; }
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

			public void AddResoure(string resource)
			{
				m_Resources.Add(resource);
			}

			public List<String> GetLocaleFiles(string locale)
			{
				return null;
			}
		}

		private class Locale
		{
			public Locale() {}
		}
	}
}

