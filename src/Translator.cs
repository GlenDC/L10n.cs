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

		public static void ImportManifest(string path)
		{
			// This is probably the only public function
			// that should be able to throw a public exception
			s_Database.Import(path);
		}

		public static void SetLocale(string id)
		{
			// TODO: for now we just let it give exceptions,
			// but we might want to make sure this can never
			// throw an exception to the outside
			// and redirect it to a warning instead.
			// In that case we probably just want to return a boolean.
			s_Database.LoadLocale(id);
		}

		public static string Translate(string id)
		{
			try {
				return s_Database.Translate(id);
			}
			catch(Exception) {
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
	}
}
