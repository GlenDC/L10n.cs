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
using L20nCore.Exceptions;

using L20nCore.Internal;

namespace L20nCore
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
		public sealed class Option<T>
		{
			public bool IsSet { get; private set; }

			private T m_Value;

			public Option()
			{
				Unset();
			}

			public Option(T value)
			{
				Set(value);
			}

			public void Set(T value)
			{
				m_Value = value;
				IsSet = m_Value != null;
			}

			public void Unset()
			{
				IsSet = false;
			}

			public T Expect(string msg)
			{
				if (!IsSet)
					throw new UnexpectedObjectException (msg);

				return m_Value;
			}


			public T Unwrap()
			{
				return Expect("cannot return an optional value when it's not set");
			}

			public Option<V> UnwrapAs<V>()
				where V : class, T
			{
				return Map((x) => new Option<V>(x as V));
			}
			
			public void UnwrapIf(UnwrapIfDelegate callback)
			{
				if (IsSet)
					callback(m_Value);
			}
			
			public Option<V> UnwrapAsOrWarning<V>(string msg, params object[] argv)
				where V : class, T
			{
				return MapOrWarning(
					(x) => new Option<V>(x as V),
					msg, argv);
			}

			public T UnwrapOr(T def)
			{
				if (!IsSet)
					return def;

				return m_Value;
			}

			public static Option<U> MapAll<U>(StaticMapOptionDelegate<U> map, params Option<T>[] options)
			{
				var argv = new T[options.Length];
				for(int i = 0; i < argv.Length; ++i) {
					if(!options[i].IsSet) {
						return new Option<U>();
					}

					argv[i] = options[i].Unwrap();
				}

				return map(argv);
			}
			
			public static Option<U> MapAllAs<U, V>
				(StaticMapAsOptionDelegate<U, V> map, params Option<T>[] options)
					where V: class, T
			{
				var argv = new V[options.Length];
				for(int i = 0; i < argv.Length; ++i) {
					var option = options[i].UnwrapAs<V>();
					if(!option.IsSet) {
						return new Option<U>();
					}
					
					argv[i] = option.Unwrap();
				}
				
				return map(argv);
			}

			public Option<U> Map<U>(MapOptionDelegate<U> map)
			{
				if (IsSet) {
					return map(m_Value);
				}

				return new Option<U>();
			}

			public Option<U> MapOrWarning<U>(MapOptionDelegate<U> map, string msg, params object[] argv)
			{
				if (IsSet) {
					return map(m_Value);
				}

				Internal.Logger.WarningFormat(msg, argv);
				return new Option<U>();
			}

			public U MapOr<U>(U def, MapDelegate<U> map)
			{
				if (IsSet) {
					return map(m_Value);
				}

				return def;
			}

			public U MapOrElse<U>(MapDelegate<U> map_if, MapDefaultDelegate<U> map_else)
			{
				if (IsSet) {
					return map_if(m_Value);
				}

				return map_else();
			}

			public Option<U> And<U>(Option<U> other)
			{
				if(IsSet)
					return other;

				return new Option<U>();
			}

			public Option<T> Or(Option<T> other)
			{
				if(IsSet)
					return this;

				return other;
			}

			public Option<T> OrElse(MapDefaultOptionDelegate<T> map_else)
			{
				if(IsSet)
					return this;

				return map_else();
			}
			
			public delegate void UnwrapIfDelegate(T value);
			public delegate U MapDelegate<U>(T value);
			public delegate Option<U> MapOptionDelegate<U>(T value);
			public delegate Option<U> StaticMapOptionDelegate<U>(params T[] value);
			public delegate Option<U> StaticMapAsOptionDelegate<U, V>(params V[] value);
			public delegate U MapDefaultDelegate<U>();
			public delegate Option<U> MapDefaultOptionDelegate<U>();
		}
	}
}
