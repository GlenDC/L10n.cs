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
	namespace Utils
	{
		/// <summary>
		/// DictionaryRef stores a reference to a dictionary.
		/// Its purpose is to provide a get interface between multiple
		/// objects without the possibility of mutation
		/// originating from those classes' calls.
		/// </summary>
		public sealed class DictionaryRef<K, V>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Utils.DictionaryRef"/> class.
			/// </summary>
			/// <param name="dictionary">reference to be stored and used</param>
			public DictionaryRef(Dictionary<K, V> dictionary)
			{
				m_Dictionary = dictionary;
			}
            
			/// <summary>
			/// Get the item linked with the specified key.
			/// </summary>
			/// <param name="key">key to look for</param>
			public V Get(K key)
			{
				return m_Dictionary [key];
			}
            
			/// <summary>
			/// Get the item linked with the specified key.
			/// If that item doesn't exist, return the default value instead.
			/// </summary>
			/// <param name="key">key to look for</param>
			/// <param name="def">default value in case key is not registered</param>
			public V Get(K key, V def)
			{
				V value;
				if (!m_Dictionary.TryGetValue(key, out value))
					value = def;

				return value;
			}

			private readonly Dictionary<K, V> m_Dictionary;
		}
	}
}
