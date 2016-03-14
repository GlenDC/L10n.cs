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

		public static string Translate(string id, params External.IVariable[] variables)
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
		
		public static void AddGlobal(string id, L20n.Objects.GlobalLiteral.Delegate callback)
		{
			s_Database.AddGlobal(id, callback);
		}
		
		public static void AddGlobal(string id, L20n.Objects.GlobalString.Delegate callback)
		{
			s_Database.AddGlobal(id, callback);
		}

		public static void SetWarningDelegate(Internal.Logger.LogDelegate callback)
		{
			Internal.Logger.SetWarningCallback(callback);
		}
	}
}
