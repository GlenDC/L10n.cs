// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

namespace L20nCore
{
	namespace Common
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
				/// Initializes a new instance of the <see cref="L20nCore.Common.Utils.DictionaryRef"/> class.
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
}
