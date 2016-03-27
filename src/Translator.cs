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

namespace L20nCore
{
	/// <summary>
	/// The public static interface for L20nCore.
	/// This is all you should need in order to have localization in your game.
	/// </summary>
	public sealed class Translator
	{
		public bool IsInitialized
		{
			get { return m_Database.IsInitialized; }
		}

		private Internal.Database m_Database;

		public Translator()
		{
			m_Database = new Internal.Database();
		}

		public List<string> Locales
		{
			get { return m_Database.Manifest.Locales; }
		}

		public string DefaultLocale
		{
			get { return m_Database.Manifest.DefaultLocale; }
		}

		public string CurrentLocale
		{
			get { return m_Database.CurrentLocale; }
		}

		/// <summary>
		/// Throws an exception if something went wrong while importing
		/// the manifest or the default locale.
		/// </summary>
		/// <param name="path">the path to the manifest file</param>
		public void ImportManifest(string path)
		{
			m_Database.Import(path);
			Internal.Logger.CurrentLocale = m_Database.CurrentLocale;
		}

		/// <summary>
		/// Throws an exception if something went wrong while importing
		/// and parsing the locale.
		/// </summary>
		/// <param name="id">the id of the locale to be loaded (as referenced in the manifest)</param>
		public void SetLocale(string id)
		{
			m_Database.LoadLocale(id);
			Internal.Logger.CurrentLocale = m_Database.CurrentLocale;
		}

		public string Translate(string id)
		{
			try
			{
				return m_Database.Translate(id);
			} catch (Exception e)
			{
				Internal.Logger.WarningFormat(
                    "A C# exception occured while translating {0}," +
					" please report this as a bug @ https://github.com/GlenDC/L20nCore.cs." +
					"\nInclude the <id> you tried to translate and all the L20n files involved; More Info: \n{1}",
                    id, e.ToString());
				return id;
			}
		}

		public string Translate(string id, string[] keys, Objects.L20nObject[] values)
		{
			try
			{
				return m_Database.Translate(id, keys, values);
			} catch (Exception e)
			{
				Internal.Logger.WarningFormat(
                    "A C# exception occured while translating {0}," +
					" please report this as a bug @ https://github.com/GlenDC/L20nCore.cs." +
					"\nInclude the <id> you tried to translate and all the L20n files involved; More Info: \n{1}",
                    id, e.ToString());
				return id;
			}
		}
        
		public string Translate(string id, string key, int value)
		{
			var keys = new string[] { key };
			var values = new Objects.L20nObject[]
                { new Objects.Literal(value) };
			return Translate(id, keys, values);
		}

		public string Translate(string id, string key, string value)
		{
			var keys = new string[] {key};
			var values = new Objects.L20nObject[]
                { new Objects.StringOutput(value) };
			return Translate(id, keys, values);
		}
        
		public string Translate(string id, string key, External.IHashValue value)
		{
			var keys = new string[] {key};
			var values = new Objects.L20nObject[]
                { new Objects.Entity(value) };
			return Translate(id, keys, values);
		}

		public void AddGlobal(string id, int value)
		{
			m_Database.AddGlobal(id, value);
		}
        
		public void AddGlobal(string id, string value)
		{
			m_Database.AddGlobal(id, value);
		}

		public void AddGlobal(string id, External.IHashValue value)
		{
			m_Database.AddGlobal(id, value);
		}
        
		public void AddGlobal(string id, Objects.DelegatedLiteral.Delegate callback)
		{
			m_Database.AddGlobal(id, callback);
		}
        
		public void AddGlobal(string id, Objects.DelegatedString.Delegate callback)
		{
			m_Database.AddGlobal(id, callback);
		}
        
		public void AddGlobal(string id, Objects.L20nObject value)
		{
			m_Database.AddGlobal(id, value);
		}

		public void SetWarningDelegate(Internal.Logger.LogDelegate callback)
		{
			Internal.Logger.SetWarningCallback(callback);
		}
	}
}
