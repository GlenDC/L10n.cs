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
	/// This is all you should need in order to have translation in your game,
	/// using your provided localization (l20n) files.
	/// </summary>
	public sealed class Translator
	{
		/// <summary>
		/// Gets a value indicating whether this instance is initialized.
		/// </summary>
		/// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
		public bool IsInitialized
		{
			get { return m_Database.IsInitialized; }
		}

		private Internal.Database m_Database;

		/// <summary>
		/// Initializes a new instance of the <see cref="L20nCore.Translator"/> class.
		/// </summary>
		public Translator()
		{
			m_Database = new Internal.Database();
		}

		/// <summary>
		/// Returns the currently available locales.
		/// </summary>
		public List<string> Locales
		{
			get { return m_Database.Manifest.Locales; }
		}

		/// <summary>
		/// Returns the current default locale, which will be used
		/// in case the <c>CurrentLocale</c> has not been set.
		/// </summary>
		public string DefaultLocale
		{
			get { return m_Database.Manifest.DefaultLocale; }
		}

		/// <summary>
		/// Gets the Current locale, which will be equal to the <c>DefaultLocale</c>,
		/// in case no other locale has been set.
		/// </summary>
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

		/// <summary>
		/// Translate the specified id within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// </summary>
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

		/// <summary>
		/// Translate the specified id using the given external variables within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// </summary>
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
        
		/// <summary>
		/// Translate the specified id using the given external <c>Literal</c> value within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// </summary>
		public string Translate(string id, string key, int value)
		{
			var keys = new string[] { key };
			var values = new Objects.L20nObject[]
                { new Objects.Literal(value) };
			return Translate(id, keys, values);
		}

		/// <summary>
		/// Translate the specified id using the given external <c>String</c> value within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// </summary>
		public string Translate(string id, string key, string value)
		{
			var keys = new string[] {key};
			var values = new Objects.L20nObject[]
                { new Objects.StringOutput(value) };
			return Translate(id, keys, values);
		}

		/// <summary>
		/// Translate the specified id using the given external <c>HashTable</c> value within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// </summary>
		public string Translate(string id, string key, External.IHashValue value)
		{
			var keys = new string[] {key};
			var values = new Objects.L20nObject[]
                { new Objects.Entity(value) };
			return Translate(id, keys, values);
		}

		/// <summary>
		/// Add the given <see cref="L20nCore.Objects.Literal"/> <c>value</c> as the global
		/// with name equal to the value of params <c>id</c>.
		/// </summary>
		public void AddGlobal(string id, int value)
		{
			m_Database.AddGlobal(id, value);
		}
        
		/// <summary>
		/// Add the given <see cref="L20nCore.Objects.StringOutput"/> <c>value</c> as the global
		/// with name equal to the value of params <c>id</c>.
		/// </summary>
		public void AddGlobal(string id, string value)
		{
			m_Database.AddGlobal(id, value);
		}

		/// <summary>
		/// Add the given <see cref="L20nCore.External.IHashValue"/> <c>value</c> as the global
		/// with name equal to the value of params <c>id</c>.
		/// </summary>
		public void AddGlobal(string id, External.IHashValue value)
		{
			m_Database.AddGlobal(id, value);
		}

		/// <summary>
		/// Add the given <see cref="L20nCore.Objects.DelegatedLiteral.Delegate"/> <c>callback</c>
		/// as the global with name equal to the value of params <c>id</c>.
		/// </summary>
		public void AddGlobal(string id, Objects.DelegatedLiteral.Delegate callback)
		{
			m_Database.AddGlobal(id, callback);
		}
        
		/// <summary>
		/// Add the given <see cref="L20nCore.Objects.DelegatedString.Delegate"/> <c>callback</c>
		/// as the global with name equal to the value of params <c>id</c>.
		/// </summary>
		public void AddGlobal(string id, Objects.DelegatedString.Delegate callback)
		{
			m_Database.AddGlobal(id, callback);
		}
        
		/// <summary>
		/// Add the given <see cref="L20nCore.Objects.L20nObject"/> <c>value</c>
		/// as the global with name equal to the value of params <c>id</c>.
		/// </summary>
		public void AddGlobal(string id, Objects.L20nObject value)
		{
			m_Database.AddGlobal(id, value);
		}

		/// <summary>
		/// Override the default logic used to log warnings
		/// with your own logic given as param <c>callback</c>.
		/// </summary>
		public void SetWarningDelegate(Internal.Logger.LogDelegate callback)
		{
			Internal.Logger.SetWarningCallback(callback);
		}
	}
}
