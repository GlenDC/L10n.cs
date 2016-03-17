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

namespace L20nCore
{
	/// <summary>
	/// The public static interface for L20nCore.
	/// This is all you should need in order to have localization in your game.
	/// </summary>
	public sealed class Translator
	{
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
			try {
				return m_Database.Translate(id);
			}
			catch(Exception e) {
				Internal.Logger.WarningFormat(
					"A C# exception occured while translating {0}," +
					" please report this as a bug @ https://github.com/GlenDC/L20nCore.cs." +
					"\nInclude the <id> you tried to translate and all the L20n files involved; More Info: \n{1}",
					id, e.ToString());
				return id;
			}
		}

		public string Translate(string id, UserVariables variables)
		{
			try {
				return m_Database.Translate(id, variables);
			}
			catch(Exception e) {
				Internal.Logger.WarningFormat(
					"A C# exception occured while translating {0}," +
					" please report this as a bug @ https://github.com/GlenDC/L20nCore.cs." +
					"\nInclude the <id> you tried to translate and all the L20n files involved; More Info: \n{1}",
					id, e.ToString());
				return id;
			}
		}

		public string Translate(string id,
		                               string parameter_key, UserVariable parameter_value)
		{
			var dic = new UserVariables(1);
			dic.Add(parameter_key, parameter_value);
			return Translate(id, dic);
		}
		
		public string Translate(string id,
		                               string parameter_key_a, UserVariable parameter_value_a,
		                               string parameter_key_b, UserVariable parameter_value_b)
		{
			var dic = new UserVariables(2);
			dic.Add(parameter_key_a, parameter_value_a);
			dic.Add(parameter_key_b, parameter_value_b);
			return Translate(id, dic);
		}
		
		public string Translate(string id,
		                               string parameter_key_a, UserVariable parameter_value_a,
		                               string parameter_key_b, UserVariable parameter_value_b,
		                               string parameter_key_c, UserVariable parameter_value_c)
		{
			var dic = new UserVariables(3);
			dic.Add(parameter_key_a, parameter_value_a);
			dic.Add(parameter_key_b, parameter_value_b);
			dic.Add(parameter_key_c, parameter_value_c);
			return Translate(id, dic);
		}
		
		public void AddGlobal(string id, UserVariable value)
		{
			m_Database.AddGlobal(id, value);
		}
		
		public void AddGlobal(string id, Objects.DelegatedObject.Delegate callback)
		{
			m_Database.AddGlobal(id, callback);
		}

		public void SetWarningDelegate(Internal.Logger.LogDelegate callback)
		{
			Internal.Logger.SetWarningCallback(callback);
		}
	}
}
