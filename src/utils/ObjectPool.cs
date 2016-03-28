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
		/// A utility class that can be used to pool objects.
		/// </summary>
		public sealed class ObjectPool<T>
			where T: new()
		{
			readonly T[] m_FreeObjects;
			int m_Size;
			int m_Capacity;
			
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Utils.DictionaryRef"/> class.
			/// </summary>
			/// <param name="size">starting size of the pool</param>
			public ObjectPool(int size)
			{
				m_FreeObjects = new T[size];
				m_Capacity = size;
				m_Size = 0;
			}

			/// <summary>
			/// Gets a free object in the pool, or create a new object otherwise.
			/// </summary>
			/// <returns>The object.</returns>
			public T GetObject()
			{
				// if a free object is available, return it
				if (m_Size > 0)
				{
					return m_FreeObjects[--m_Size];
				}

				// return it otherwise
				return new T();
			}

			/// <summary>
			/// Give the object back to the pool.
			/// Make sure that the object has been cleaned up,
			/// as that will not happen for you.
			/// </summary>
			public void ReturnObject(ref T obj)
			{
				if (m_Size < m_Capacity)
				{
					m_FreeObjects[m_Size++] = obj;
				}

				obj = default(T);
			}
		}
	}
}
