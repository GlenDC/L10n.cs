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
using L20n.Exceptions;

using L20n.Internal;

namespace L20n
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

			public V UnwrapAs<V>() where V : T
			{
				try {
					return (V)Unwrap();
				}
				catch(Exception e) {
					var msg = String.Format(
						"object could not be given as {0}", typeof(V));
					throw new UnexpectedObjectException(msg, e);
				}
			}

			public T UnwrapOr(T def)
			{
				if (!IsSet)
					return def;

				return m_Value;
			}

			public delegate U MapDelegate<U>(T value);
			public delegate Option<U> MapOptionDelegate<U>(T value);
			public delegate Option<U> StaticMapOptionDelegate<U>(params T[] value);
			public delegate U MapDefaultDelegate<U>();
			public delegate Option<U> MapDefaultOptionDelegate<U>();

			public static Option<U> Map<U>(StaticMapOptionDelegate<U> map, params Option<T>[] options)
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
		}
	}
}
