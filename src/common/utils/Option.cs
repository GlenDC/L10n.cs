// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common;
using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace Common
	{
		namespace Utils
		{
			/// <summary>
			/// A class that allows you to wrap another nullable object.
			/// This class should be used to prevent the use of null within the
			/// rest of the code base.
			///
			/// It is heavily inspired on std::option from Rust:
			/// https://doc.rust-lang.org/std/option/enum.Option.html
			/// </summary>
			/// <remarks>
			/// This class seemed like a good idea, but using it in c# is just expensive because
			/// of the GC. I suppose it works in Rust well, because we're purely allocating stuff on the stack.
			/// Sadly this is impossible and/or akward in C#, so we'll have to stick with null
			/// for performance reasons, sadly. If somebody reading this thinks he/she has a solution, let me know.
			/// </remarks>
			public sealed class Option<T>
			{
				/// <summary>
				/// Gets a value indicating whether this option has a value.
				/// </summary>
				/// <value><c>true</c> if this option has avalue; otherwise, <c>false</c>.</value>
				public bool IsSet { get; private set; }

				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.Common.Utils.Option"/> class,
				///  with no value set.
				/// </summary>
				public Option()
				{
					Unset();
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.Common.Utils.Option"/> class,
				///  with the given value as the option value.
				/// </summary>
				/// <param name="value">value to use as the option value</param>
				public Option(T value)
				{
					Set(value);
				}

				/// <summary>
				/// Modify the currently set option value.
				/// </summary>
				/// <param name="value">value to use as the new option value</param>
				/// <remarks>
				/// In case the value-type is a class and the value is null, 
				/// this option will become a none-option.
				/// </remarks>
				public void Set(T value)
				{
					m_Value = value;
					IsSet = m_Value != null;
				}

				/// <summary>
				/// Unset this option, making it a none-option.
				/// </summary>
				public void Unset()
				{
					IsSet = false;
				}

				/// <summary>
				/// Expect a value to be set and return it.
				/// Throws a <see cref="L20nCore.Exceptions.UnexpectedObjectException"/> 
				/// if this option has not been set.
				/// </summary>
				/// <param name="msg">exception-message in case this option has no value</param>
				public T Expect(string msg)
				{
					if (!IsSet)
						throw new UnexpectedObjectException(msg);

					return m_Value;
				}

				/// <summary>
				/// Unwrap the value, if no value had been set,
				/// a <see cref="L20nCore.Exceptions.UnexpectedObjectException"/> will be thrown. 
				/// </summary>
				public T Unwrap()
				{
					return Expect("cannot return an optional value when it's not set");
				}

				/// <summary>
				/// Unwrap the value as type <c>V</c>, if no value had been set,
				/// a <see cref="L20nCore.Exceptions.UnexpectedObjectException"/> will be thrown. 
				/// </summary>
				public Option<V> UnwrapAs<V>()
                where V : class, T
				{
					return Map((x) => new Option<V>(x as V));
				}
            
				/// <summary>
				/// Calls the given callback with the value that has been set.
				/// If no value has been set, nothing happens.
				/// </summary>
				/// <param name="callback">delegate to be called in case a value has been set</param>
				public void UnwrapIf(UnwrapIfDelegate callback)
				{
					if (IsSet)
						callback(m_Value);
				}
            
				/// <summary>
				/// Maps this option to an optional value of type <c>V</c>, with the current value
				/// casted to <c>V</c> if possible, or with no value set otherwise.
				/// </summary>
				/// <returns>an optional value of type <c>V</c></returns>
				/// <param name="msg">the message to be given as a warning if no value has been set or in case
				/// the value of this option could not be cast to type <c>V</c></param>
				/// <param name="argv">optional objects to be used in the formatted <c>msg</c></param>
				public Option<V> UnwrapAsOrWarning<V>(string msg, params object[] argv)
                where V : class, T
				{
					return MapOrWarning(
                    (x) => new Option<V>(x as V),
                    msg, argv);
				}

				/// <summary>
				/// Returns the value if one has been set, returns <c>def</c> otheriwse.
				/// </summary>
				public T UnwrapOr(T def)
				{
					if (!IsSet)
						return def;

					return m_Value;
				}

				/// <summary>
				/// Calls the given delegate <c>map</c> with the optional values <c>options</c>
				/// unwrapped if possible. Returns a non-option of type <c>U</c> otherwise.
				/// </summary>
				public static Option<U> MapAll<U>(StaticMapOptionDelegate<U> map, params Option<T>[] options)
				{
					var argv = new T[options.Length];
					for (int i = 0; i < argv.Length; ++i)
					{
						if (!options [i].IsSet)
						{
							return new Option<U>();
						}

						argv [i] = options [i].Unwrap();
					}

					return map(argv);
				}
            
				/// <summary>
				/// Calls the given delegate <c>map</c> with the optional values <c>options</c>
				/// unwrapped and cast to type <c>V</c> if possible.
				/// Returns a non-option of type <c>U</c> otherwise.
				/// </summary>
				public static Option<U> MapAllAs<U, V>
                (StaticMapAsOptionDelegate<U, V> map, params Option<T>[] options)
                    where V: class, T
				{
					var argv = new V[options.Length];
					for (int i = 0; i < argv.Length; ++i)
					{
						var option = options [i].UnwrapAs<V>();
						if (!option.IsSet)
						{
							return new Option<U>();
						}
                    
						argv [i] = option.Unwrap();
					}
                
					return map(argv);
				}

				/// <summary>
				/// Returns the optional value of the call to the given delegate <c>map</c>
				/// in case a value in this option has been set, returns a non-option otheriwse.
				/// </summary>
				public Option<U> Map<U>(MapOptionDelegate<U> map)
				{
					if (IsSet)
					{
						return map(m_Value);
					}

					return new Option<U>();
				}

				/// <summary>
				/// Returns the optional value of the call to the given delegate <c>map</c>
				/// in case a value in this option has been set.
				/// Logs a warning with the given <c>msg</c> otherwise and returns a non-option.
				/// </summary>
				public Option<U> MapOrWarning<U>(MapOptionDelegate<U> map, string msg, params object[] argv)
				{
					if (IsSet)
					{
						return map(m_Value);
					}

					Logger.WarningFormat(msg, argv);
					return new Option<U>();
				}

				/// <summary>
				/// Returns the value of the call to the given delegate <c>map</c>
				/// in case a value in this option has been set. Returns <c>def</c> otherwise.
				/// </summary>
				public U MapOr<U>(U def, MapDelegate<U> map)
				{
					if (IsSet)
					{
						return map(m_Value);
					}

					return def;
				}

				/// <summary>
				/// Calls <c>map_if</c> with the given value if one has been set,
				/// calls <c>map_else</c> otherwise. Returns the value of the called delegate.
				/// </summary>
				public U MapOrElse<U>(MapDelegate<U> map_if, MapDefaultDelegate<U> map_else)
				{
					if (IsSet)
					{
						return map_if(m_Value);
					}

					return map_else();
				}

				/// <summary>
				/// returns <c>other</c> in case both <c>this</c> and <c>other</c> have been set,
				/// returns a none option otherwise.
				/// </summary>
				public Option<U> And<U>(Option<U> other)
				{
					if (IsSet)
						return other;

					return new Option<U>();
				}

				/// <summary>
				/// returns <c>this</c> in case this option has a value set,
				/// returns <c>other</c> otherwise.
				/// </summary>
				public Option<T> Or(Option<T> other)
				{
					if (IsSet)
						return this;

					return other;
				}
				/// <summary>
				/// returns <c>this</c> in case this option has a value set,
				/// returns the return value of a call to the given delegate <c>map_else</c> otherwise.
				/// </summary>
				public Option<T> OrElse(MapDefaultOptionDelegate<T> map_else)
				{
					if (IsSet)
						return this;

					return map_else();
				}
            
				public delegate void UnwrapIfDelegate(T value);

				public delegate U MapDelegate<U>(T value);

				public delegate Option<U> MapOptionDelegate<U>(T value);

				public delegate Option<U> StaticMapOptionDelegate<U>(params T[] value);

				public delegate Option<U> StaticMapAsOptionDelegate<U,V>(params V[] value);

				public delegate U MapDefaultDelegate<U>();

				public delegate Option<U> MapDefaultOptionDelegate<U>();
			
				private T m_Value;
			}
		}
	}
}
