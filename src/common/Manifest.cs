// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Collections.Generic;

using SimpleJSON;

using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace Common
	{
		/// <summary>
		/// A class that is a C# mapping over the actual L20n Manifest file.
		/// </summary>
		public sealed class Manifest
		{
			/// <summary>
			/// Get the locales listed in this manifest.
			/// </summary>
			public List<string> Locales
			{
				get { return m_Locales; }
			}

			/// <summary>
			/// Get the locale marked as default in this manifest.
			/// Throws a warning if it's not listed as a locale.
			/// </summary>
			public string DefaultLocale
			{
				get { return m_DefaultLocale; }
				private set
				{
					if (!m_Locales.Contains(value))
					{
						string msg = string.Format("{0} is not a valid default locale as " +
							"it couldn't be found in the list of locales.", value);
						throw new ImportException(msg);
					}

					m_DefaultLocale = value;
				}
			}

			/// <summary>
			/// Gets the version as specified by this Version.
			/// </summary>
			/// <remarks>
			/// When no version is specified, the default version is used.
			/// </remarks>
			public Version Version
			{
				get { return m_Version; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Common.Manifest"/> class.
			/// </summary>
			public Manifest()
			{
				m_Locales = new List<string>();
				m_DefaultLocale = null;
				m_Resources = new List<string>();
				m_Version = Version.L20n;
			}

			/// <summary>
			/// Replace the current information with
			/// the manifest content found at the specified manifest_path.
			/// </summary>
			public void Import(string manifest_path)
			{
				Flush();

				try
				{
					using (var sr = IO.StreamReaderFactory.Create(manifest_path))
					{
						var json = sr.ReadToEnd();
						var root = JSON.Parse(json);

						var locales = root ["locales"];
						if (locales == null || locales.Count == 0)
						{
							string msg = string.Format("No locales were provided in: {0}", manifest_path);
							throw new ImportException(msg);
						}

						foreach (var locale in locales.Children)
						{
							m_Locales.Add(locale.Value);
						}

						var defaultLocale = root ["default_locale"].Value;
						if (defaultLocale == "")
						{
							string msg = string.Format("No default locale was provided in: {0}", manifest_path);
							throw new ImportException(msg);
						}

						DefaultLocale = defaultLocale;

						var resources = root ["resources"];
						if (resources == null || resources.Count == 0)
						{
							string msg = string.Format("No resources were provided in: {0}", manifest_path);
							throw new ImportException(msg);
						}

						foreach (var resource in resources.Children)
						{
							AddResoure(resource.Value, manifest_path);
						}

						var version = root ["version"].Value;
						if (version != "")
						{
							m_Version = Versions.FromString(version);
						}
					}
				} catch (FileNotFoundException)
				{
					string msg = string.Format("manifest file couldn't be found: {0}", manifest_path);
					throw new ImportException(msg);
				}
			}

			/// <summary>
			/// Get all the locale files available for the given <c>locale</c>.
			/// </summary>
			public List<String> GetLocaleFiles(string locale)
			{
				var files = new List<String>();
				foreach (var resource in m_Resources)
				{
					var file = resource.Replace(LOCALE_STRING_ID, locale);
					files.Add(file);
				}

				return files;
			}

			private void Flush()
			{
				m_Locales.Clear();
				m_DefaultLocale = null;
				m_Resources.Clear();
			}

			private void AddResoure(string resource, string manifest)
			{
				if (!resource.Contains(LOCALE_STRING_ID))
				{
					string msg = string.Format("Resource '{0}' does not contain local string-id ({1}).",
					                           resource, LOCALE_STRING_ID);
					throw new ImportException(msg);
				}

				resource = manifest.Replace(Path.GetFileName(manifest), resource);
				m_Resources.Add(resource);
			}

			private const string LOCALE_STRING_ID = "{{locale}}";
			private List<string> m_Locales;
			private string m_DefaultLocale;
			private List<string> m_Resources;
			private Version m_Version;
		}
	}
}
