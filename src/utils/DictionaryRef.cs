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

namespace L20nCore
{
	namespace Utils
	{
		public sealed class DictionaryRef<K, V>
		{
			private readonly Dictionary<K, V> m_Dictionary;

			public DictionaryRef(Dictionary<K, V> dictionary)
			{
				m_Dictionary = dictionary;
			}
			
			public V Get(K key)
			{
				return m_Dictionary[key];
			}
			
			public V Get(K key, V def)
			{
				V value;
				if (!m_Dictionary.TryGetValue(key, out value))
					value = def;

				return value;
			}
		}
	}
}
