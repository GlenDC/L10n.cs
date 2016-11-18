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
			/// A utility class that can be used to pool objects.
			/// </summary>
			public sealed class ObjectPool<T>
			where T: new()
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.Common.Utils.DictionaryRef"/> class.
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
				public T GetObject()
				{
					// if a free object is available, return it
					if (m_Size > 0)
					{
						return m_FreeObjects [--m_Size];
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
						m_FreeObjects [m_Size++] = obj;
					}

					obj = default(T);
				}

				private readonly T[] m_FreeObjects;
				private int m_Size;
				private int m_Capacity;
			}
		}
	}
}
