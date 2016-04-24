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

using L20nCore.Exceptions;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Internal
	{
		public sealed class Database
		{
			/// <summary>
			/// Gets a value indicating whether this instance is initialized.
			/// </summary>
			/// <value>
			/// <c>true</c> if this instance is initialized;
			/// otherwise, <c>false</c>.
			/// </value>
			public bool IsInitialized
			{
				get { return CurrentLocale != null; }
			}

			/// <summary>
			/// Returns the <see cref="L20nCore.Internal.Manifest"/>
			/// instance linked with this Database.
			/// </summary>
			public Manifest Manifest { get; private set; }

			/// <summary>
			/// Returns the Currently set Locale.
			/// This value will be equal to the <c>Manifest.DefaultLocale</c>
			/// in case there is locale set other than the default one.
			/// </summary>
			public string CurrentLocale { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Internal.Database"/> class.
			/// </summary>
			public Database()
			{
				Manifest = new Manifest();
				m_Globals = new Dictionary<string, Objects.L20nObject>();

				m_DummyObject = new Objects.Dummy();

				CurrentLocale = null;

				m_DefaultContext = null;
				m_CurrentContext = null;

				AddSystemGlobals();
			}

			/// <summary>
			/// Initialize the linked <see cref="L20nCore.Internal.Manifest"/> instance
			/// using the manifest file found at <c>manifest_path</c>.
			/// The default locale will also be fully imported and parsed as a last step.
			/// </summary>
			public void Import(string manifest_path)
			{
				m_DefaultContext = null;
				m_CurrentContext = null;

				Manifest.Import(manifest_path);
				ImportLocal(Manifest.DefaultLocale, out m_DefaultContext, null);

				CurrentLocale = Manifest.DefaultLocale;
			}

			/// <summary>
			/// In case the locale is eequal to <c>Manifest.DefaultLocale</c>,
			/// it will be fully loaded and parsed and used as the Current Locale (preference) for translations.
			/// Otherwise the default locale that's loaded since the beginning will be used instead.
			/// A <see cref="L20nCore.Exceptions.ImportException"/> gets thrown in case <c>id</c> is equal to <c>null</c>.
			/// </summary>
			public void LoadLocale(string id)
			{
				if (id == null)
				{
					throw new ImportException(
						"a locale id has to be given in order to load one");
				}

				if (id == Manifest.DefaultLocale)
				{
					m_CurrentContext = null;
					CurrentLocale = Manifest.DefaultLocale;
				} else if (IsInitialized)
				{
					ImportLocal(id, out m_CurrentContext, m_DefaultContext);
					CurrentLocale = id;
				} else
				{
					throw new ImportException(
						"couldn't load locale as the L20n databse has no been initialized");
				}
			}

			/// <summary>
			/// Translate the specified id within the currently set locale,
			/// returns the id itself in case no entity could be matched with the given id.
			/// </summary>
			public string Translate(string id)
			{
				return TranslateID(id);
			}

			/// <summary>
			/// Translate the specified id using the given external variables within the currently set locale,
			/// returns the id itself in case no entity could be matched with the given id.
			/// </summary>
			public string Translate(string id, string[] keys, Objects.L20nObject[] values)
			{
				// we expect at least external value here,
				// and the amount of keys need to be equal to the amount of values
				// for obvious reasons.
				if (keys.Length == 0)
				{
					Internal.Logger.WarningFormat(
						"couldn't translate {0} as no keys were given", id);
					return id;
				}
				if (keys.Length != values.Length)
				{
					Internal.Logger.WarningFormat(
						"couldn't translate {0} as the amount of keys and values isn't equal," +
						" expected {1}, got {2}", id, keys.Length, values.Length);
					return id;
				}

				// Get either the current or default locale, throw a warning otherwise.
				var ctx = m_CurrentContext != null ? m_CurrentContext : m_DefaultContext;
				if (ctx == null)
				{
					Logger.WarningFormat("couldn't translate {0} as no context has been set", id);
					return id;
				}

				// push all variables to the stack
				for (int i = 0; i < keys.Length; ++i)
				{
					if (keys [i] == null)
					{
						Logger.WarningFormat("couldn't translate {0} because parameter-key #{1} is null" +
							" while expecting an string", id, i);
						break;
					}

					if (values [i] == null)
					{
						Logger.WarningFormat("couldn't translate {0} because parameter-value #{1} is null",
						                     id, i);
						break;
					}

					ctx.PushVariable(keys [i], values [i]);
				}

				// translate the acutal id, with the variables now available on the stack
				var output = TranslateID(id);

				// remove variables from stack again
				for (int i = 0; i < keys.Length; ++i)
				{
					ctx.DropVariable(keys [i]);
				}

				// return the translated string
				return output;
			}

			/// <summary>
			/// Add the given <see cref="L20nCore.Objects.Literal"/> <c>value</c> as the global
			/// with name equal to the value of params <c>id</c>.
			/// </summary>
			public void AddGlobal(string id, int value)
			{
				AddGlobal(id, new Objects.Literal(value));
			}

			/// <summary>
			/// Add the given <see cref="L20nCore.Objects.StringOutput"/> <c>value</c> as the global
			/// with name equal to the value of params <c>id</c>.
			/// </summary>
			public void AddGlobal(string id, string value)
			{
				AddGlobal(id, new Objects.StringOutput(value));
			}

			/// <summary>
			/// Add the given <see cref="L20nCore.Objects.BooleanValue"/> <c>value</c> as the global
			/// with name equal to the value of params <c>id</c>.
			/// </summary>
			public void AddGlobal(string id, bool value)
			{
				AddGlobal(id, new Objects.BooleanValue(value));
			}

			/// <summary>
			/// Add the given <see cref="L20nCore.External.IHashValue"/> <c>value</c> as the global
			/// with name equal to the value of params <c>id</c>.
			/// </summary>
			public void AddGlobal(string id, External.IHashValue value)
			{
				AddGlobal(id, new Objects.Entity(value));
			}

			/// <summary>
			/// Add the given <see cref="L20nCore.Objects.DelegatedLiteral+Delegate"/> <c>callback</c>
			/// as the global with name equal to the value of params <c>id</c>.
			/// </summary>
			public void AddGlobal(string id, Objects.DelegatedLiteral.Delegate callback)
			{
				AddGlobal(id, new Objects.DelegatedLiteral(callback));
			}

			/// <summary>
			/// Add the given <see cref="L20nCore.Objects.DelegatedString+Delegate"/> <c>callback</c>
			/// as the global with name equal to the value of params <c>id</c>.
			/// </summary>
			public void AddGlobal(string id, Objects.DelegatedString.Delegate callback)
			{
				AddGlobal(id, new Objects.DelegatedString(callback));
			}
			
			/// <summary>
			/// Add the given <see cref="L20nCore.Objects.DelegatedBoolean+Delegate"/> <c>callback</c>
			/// as the global with name equal to the value of params <c>id</c>.
			/// </summary>
			public void AddGlobal(string id, Objects.DelegatedBoolean.Delegate callback)
			{
				AddGlobal(id, new Objects.DelegatedBoolean(callback));
			}

			/// <summary>
			/// Add the given <see cref="L20nCore.Objects.L20nObject"/> <c>value</c>
			/// as the global with name equal to the value of params <c>id</c>.
			/// </summary>
			public void AddGlobal(string id, Objects.L20nObject value)
			{
				if (value == null)
				{
					Logger.WarningFormat(
						"global user-variable {0} cannot be a null-value", id);
					return;
				}

				try
				{
					m_Globals.Add(id, value);
				} catch (ArgumentException)
				{
					Logger.WarningFormat(
						"global value with id {0} isn't unique, " +
						"and old value will be overriden", id);
					m_Globals [id] = value;
				}
			}

			private void ImportLocal(string id, out LocaleContext context, LocaleContext parent)
			{
				// Request the locale files from the manifest based on the given id
				var localeFiles = Manifest.GetLocaleFiles(id);
				if (localeFiles.Count == 0)
				{
					string msg = string.Format("No resources were found for locale: {0}", id);
					throw new Exceptions.ImportException(msg);
				}

				// build & return the given context.
				var builder = new LocaleContext.Builder();
				for (var i = 0; i < localeFiles.Count; ++i)
				{
					builder.Import(localeFiles [i]);
				}

				context = builder.Build(m_Globals, parent);
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

			private string TranslateID(string id)
			{
				Objects.L20nObject identifier;

				// the given identifier is either a property or simply an identifier
				if (id.IndexOf('.') > 0)
					identifier = new Objects.PropertyExpression(id.Split('.'));
				else
					identifier = new Objects.IdentifierExpression(id);

				// get the current locale which can be default locale,
				// a warning gets thrown in case no context was set.
				var context = m_CurrentContext != null ? m_CurrentContext : m_DefaultContext;
				if (context == null)
				{
					Logger.WarningFormat(
						"{0} could not be translated as no language-context has been set", id);
					return id;
				}

				// evaluate the givent identifier, with the current context,
				// using m_DummyContext to tell the evaluation that we shouldn't
				// be allowed to access entities at the higest level (1st level).
				var output = identifier.Eval(context, m_DummyObject)
					as Objects.StringOutput;

				// if output is null, it means we failed the translation,
				// and a warning will be thrown instead.
				if (output == null)
				{
					Internal.Logger.WarningFormat(
						"something went wrong, {0} could not be translated", id);
					return id;
				}

				// Otheriwse simply return the value extracted
				// from the outputted StringOuput object.
				return output.Value;
			}

			private LocaleContext m_DefaultContext;
			private LocaleContext m_CurrentContext;
			private Objects.Dummy m_DummyObject;
			private readonly Dictionary<string, Objects.L20nObject> m_Globals;
		}
	}
}
