// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using L20nCore.Internal;
using L20nCore.Exceptions;

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
			get { return CurrentLocale != null; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="L20nCore.Translator"/> class.
		/// </summary>
		public Translator()
		{
			m_Manifest = new Manifest();
			m_Globals = new Dictionary<string, Objects.L20nObject>();
			
			m_DummyObject = new Objects.Dummy();
			
			CurrentLocale = null;
			
			m_DefaultContext = null;
			m_CurrentContext = null;
			
			AddSystemGlobals();
		}

		/// <summary>
		/// Returns the currently available locales.
		/// </summary>
		public List<string> Locales
		{
			get { return m_Manifest.Locales; }
		}

		/// <summary>
		/// Returns the current default locale, which will be used
		/// in case the <c>CurrentLocale</c> has not been set.
		/// </summary>
		public string DefaultLocale
		{
			get { return m_Manifest.DefaultLocale; }
		}

		/// <summary>
		/// Gets the Current locale, which will be equal to the <c>DefaultLocale</c>,
		/// in case no other locale has been set.
		/// </summary>
		public string CurrentLocale { get; private set; }

		/// <summary>
		/// Throws an exception if something went wrong while importing
		/// the manifest or the default locale.
		/// </summary>
		/// <param name="path">the path to the manifest file</param>
		public void ImportManifest(string path)
		{
			m_DefaultContext = null;
			m_CurrentContext = null;
			
			m_Manifest.Import(path);
			ImportLocal(m_Manifest.DefaultLocale, out m_DefaultContext, null);
			
			CurrentLocale = m_Manifest.DefaultLocale;
	
			Internal.Logger.CurrentLocale = CurrentLocale;
		}

		/// <summary>
		/// Throws an exception if something went wrong while importing
		/// and parsing the locale.
		/// </summary>
		/// <param name="id">the id of the locale to be loaded (as referenced in the manifest)</param>
		public void SetLocale(string id)
		{
			if (id == null)
			{
				throw new ImportException(
					"a locale id has to be given in order to load one");
			}
			
			if (id == m_Manifest.DefaultLocale)
			{
				m_CurrentContext = null;
				CurrentLocale = m_Manifest.DefaultLocale;
			} else if (IsInitialized)
			{
				ImportLocal(id, out m_CurrentContext, m_DefaultContext);
				CurrentLocale = id;
			} else
			{
				throw new ImportException(
					"couldn't load locale as the L20n databse has no been initialized");
			}

			Internal.Logger.CurrentLocale = CurrentLocale;
		}

		/// <summary>
		/// Translate the specified id within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id)
		{
			try
			{
				return TranslateID(id);
			} catch (Exception e)
			{
				Internal.Logger.WarningFormat(TRANSLATION_EXCEPTION_MESSAGE, id, e.ToString());
				return id;
			}
		}

		/// <summary>
		/// Translate the specified id using the given external variables within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, List<string> keys, List<Objects.L20nObject> values)
		{
			try
			{
				// we expect at least external value here,
				// and the amount of keys need to be equal to the amount of values
				// for obvious reasons.
				if (keys.Count == 0)
				{
					Internal.Logger.WarningFormat(
						"couldn't translate {0} as no keys were given", id);
					return id;
				}
				if (keys.Count != values.Count)
				{
					Internal.Logger.WarningFormat(
						"couldn't translate {0} as the amount of keys and values isn't equal," +
						" expected {1}, got {2}", id, keys.Count, values.Count);
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
				for (int i = 0; i < keys.Count; ++i)
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
				for (int i = 0; i < keys.Count; ++i)
				{
					ctx.DropVariable(keys [i]);
				}
				
				// return the translated string
				return output;
			} catch (Exception e)
			{
				Internal.Logger.WarningFormat(TRANSLATION_EXCEPTION_MESSAGE, id, e.ToString());
				return id;
			}
		}
		
		/// <summary>
		/// Translate the specified id using the given external <c>Boolean</c> value within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key, bool value)
		{
			return Translate(id, key, new Objects.BooleanValue(value));
		}

		/// <summary>
		/// Translate the specified id using the given external <c>Literal</c> value within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key, int value)
		{
			return Translate(id, key, new Objects.Literal(value));
		}

		/// <summary>
		/// Translate the specified id using the given external <c>String</c> value within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key, string value)
		{
			return Translate(id, key, new Objects.StringOutput(value));
		}

		/// <summary>
		/// Translate the specified id using the given external <c>HashTable</c> value within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key, External.IHashValue value)
		{
			return Translate(id, key, new Objects.Entity(value));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, int value_a, int value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.Literal(value_a), new Objects.Literal(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, int value_a, bool value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.Literal(value_a), new Objects.BooleanValue(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, int value_a, string value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.Literal(value_a), new Objects.StringOutput(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, int value_a, External.IHashValue value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.Literal(value_a), new Objects.Entity(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, bool value_a, bool value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.BooleanValue(value_a), new Objects.BooleanValue(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, bool value_a, int value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.BooleanValue(value_a), new Objects.Literal(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, bool value_a, string value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.BooleanValue(value_a), new Objects.StringOutput(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, bool value_a, External.IHashValue value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.BooleanValue(value_a), new Objects.Entity(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, string value_a, string value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.StringOutput(value_a), new Objects.StringOutput(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, string value_a, bool value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.StringOutput(value_a), new Objects.BooleanValue(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, string value_a, int value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.StringOutput(value_a), new Objects.Literal(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, string value_a, External.IHashValue value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.StringOutput(value_a), new Objects.Entity(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, External.IHashValue value_a, External.IHashValue value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.Entity(value_a), new Objects.Entity(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, External.IHashValue value_a, bool value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.Entity(value_a), new Objects.BooleanValue(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, External.IHashValue value_a, int value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.Entity(value_a), new Objects.Literal(value_b));
		}
		
		/// <summary>
		/// Translate the specified id using the given external values within the currently set locale,
		/// returns the id itself in case no entity could be matched with the given id.
		/// Any <see cref="System.Exception"/> gets captured and turned into an unexpected warning.
		/// </summary>
		public string Translate(string id, string key_a, string key_b, External.IHashValue value_a, string value_b)
		{
			return Translate(id, key_a, key_b,
			                 new Objects.Entity(value_a), new Objects.StringOutput(value_b));
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

		/// <summary>
		/// Override the default logic used to log warnings
		/// with your own logic given as param <c>callback</c>.
		/// </summary>
		public void SetWarningDelegate(Internal.Logger.LogDelegate callback)
		{
			Internal.Logger.SetWarningCallback(callback);
		}

		private string Translate(string id, string key_a, Objects.L20nObject value_a)
		{
			try
			{	
				// Get either the current or default locale, throw a warning otherwise.
				var ctx = m_CurrentContext != null ? m_CurrentContext : m_DefaultContext;
				if (ctx == null)
				{
					Logger.WarningFormat("couldn't translate {0} as no context has been set", id);
					return id;
				}

				if (key_a == null) {
					Logger.Warning("couldn't translate {0} because a given parameter-key is null");
					return id;
				}
				
				if (value_a == null) {
					Logger.Warning("couldn't translate {0} because a given parameter-value is null");
					return id;
				}

				// push variables to the stack
				ctx.PushVariable(key_a, value_a);
				
				// translate the acutal id, with the variables now available on the stack
				var output = TranslateID(id);
				
				// remove variables from stack again
				ctx.DropVariable(key_a);
				
				// return the translated string
				return output;
			} catch (Exception e)
			{
				Internal.Logger.WarningFormat(TRANSLATION_EXCEPTION_MESSAGE, id, e.ToString());
				return id;
			}
		}
		
		private string Translate(string id, string key_a, string key_b, Objects.L20nObject value_a, Objects.L20nObject value_b)
		{
			try
			{	
				// Get either the current or default locale, throw a warning otherwise.
				var ctx = m_CurrentContext != null ? m_CurrentContext : m_DefaultContext;
				if (ctx == null)
				{
					Logger.WarningFormat("couldn't translate {0} as no context has been set", id);
					return id;
				}
				
				if (key_a == null || key_b == null) {
					Logger.Warning("couldn't translate {0} because a given parameter-key is null");
					return id;
				}
				
				if (value_a == null || value_b == null) {
					Logger.Warning("couldn't translate {0} because a given parameter-value is null");
					return id;
				}
				
				// push variables to the stack
				ctx.PushVariable(key_a, value_a);
				ctx.PushVariable(key_b, value_b);
				
				// translate the acutal id, with the variables now available on the stack
				var output = TranslateID(id);
				
				// remove variables from stack again
				ctx.DropVariable(key_b);
				ctx.DropVariable(key_a);
				
				// return the translated string
				return output;
			} catch (Exception e)
			{
				Internal.Logger.WarningFormat(TRANSLATION_EXCEPTION_MESSAGE, id, e.ToString());
				return id;
			}
		}
		
		private void ImportLocal(string id, out LocaleContext context, LocaleContext parent)
		{
			// Request the locale files from the manifest based on the given id
			var localeFiles = m_Manifest.GetLocaleFiles(id);
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

		private Manifest m_Manifest;
		private LocaleContext m_DefaultContext;
		private LocaleContext m_CurrentContext;
		private Objects.Dummy m_DummyObject;
		private readonly Dictionary<string, Objects.L20nObject> m_Globals;
		
		private const string TRANSLATION_EXCEPTION_MESSAGE =
			"A C# exception occured while translating {0}," +
				" please report this as a bug @ https://github.com/GlenDC/L20nCore.cs." +
				"\nInclude the <id> you tried to translate and all the L20n files involved; More Info: \n{1}";
	}
}
