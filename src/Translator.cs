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

namespace L20n
{
	/// <summary>
	/// The public static interface for L20n.
	/// This is all you should need in order to have localization in your game.
	/// </summary>
	public static class Translator
	{
		private static Internal.Database s_Database = new Internal.Database();

		public static List<string> Locales
		{
			get { return s_Database.Manifest.Locales; }
		}

		public static string DefaultLocale
		{
			get { return s_Database.Manifest.DefaultLocale; }
		}

		public static string CurrentLocale
		{
			get { return s_Database.CurrentLocale; }
		}

		/// <summary>
		/// Throws an exception if something went wrong while importing
		/// the manifest or the default locale.
		/// </summary>
		/// <param name="path">the path to the manifest file</param>
		public static void ImportManifest(string path)
		{
			s_Database.Import(path);
		}

		/// <summary>
		/// Throws an exception if something went wrong while importing
		/// and parsing the locale.
		/// </summary>
		/// <param name="id">the id of the locale to be loaded (as referenced in the manifest)</param>
		public static void SetLocale(string id)
		{
			s_Database.LoadLocale(id);
		}

		public static string Translate(string id)
		{
			try {
				return s_Database.Translate(id);
			}
			catch(Exception e) {
				Internal.Logger.WarningFormat(
					"A C# exception occured while translating {0}," +
					" please report this as a bug @ https://github.com/GlenDC/L20n.cs." +
					"\nInclude the <id> you tried to translate and all the L20n files involved; More Info: \n{1}",
					id, e.ToString());
				return id;
			}
		}

		public static string Translate(string id, UserVariables variables)
		{
			try {
				return s_Database.Translate(id, variables);
			}
			catch(Exception e) {
				Internal.Logger.WarningFormat(
					"A C# exception occured while translating {0}," +
					" please report this as a bug @ https://github.com/GlenDC/L20n.cs." +
					"\nInclude the <id> you tried to translate and all the L20n files involved; More Info: \n{1}",
					id, e.ToString());
				return id;
			}
		}

		public static string Translate(string id,
		                               string parameter_key, UserVariable parameter_value)
		{
			var dic = new UserVariables(1);
			dic.Add(parameter_key, parameter_value);
			return Translate(id, dic);
		}
		
		public static string Translate(string id,
		                               string parameter_key,
		                               Objects.StringOutputCallback.Delegate parameter_value)
		{
			return Translate(id, parameter_key, new UserVariable(parameter_value));
		}
		
		public static string Translate(string id,
		                               string parameter_key,
		                               Objects.LiteralCallback.Delegate parameter_value)
		{
			return Translate(id, parameter_key, new UserVariable(parameter_value));
		}
		
		public static string Translate(string id,
		                               string parameter_key_a, UserVariable parameter_value_a,
		                               string parameter_key_b, UserVariable parameter_value_b)
		{
			var dic = new UserVariables(2);
			dic.Add(parameter_key_a, parameter_value_a);
			dic.Add(parameter_key_b, parameter_value_b);
			return Translate(id, dic);
		}
		
		public static string Translate(string id,
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
		
		public static void AddGlobal(string id, UserVariable value)
		{
			s_Database.AddGlobal(id, value);
		}
		
		public static void AddGlobal(string id, Objects.LiteralCallback.Delegate callback)
		{
			s_Database.AddGlobal(id, callback);
		}
		
		public static void AddGlobal(string id, Objects.StringOutputCallback.Delegate callback)
		{
			s_Database.AddGlobal(id, callback);
		}

		public static void SetWarningDelegate(Internal.Logger.LogDelegate callback)
		{
			Internal.Logger.SetWarningCallback(callback);
		}
	}
}
