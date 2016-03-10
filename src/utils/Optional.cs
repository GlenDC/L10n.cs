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
		/// It is heavily inspired on many older (functional) languages out there.
		/// </summary>
		public sealed class Optional<T> where T: class
		{
			public bool IsSet
			{
				get { return m_Value != null; }
			}
			private T m_Value;

			public Optional(T value = null)
			{
				Set(value);
			}

			public void Set(T value)
			{
				m_Value = value;
			}
			
			public T Expect(string msg = null)
			{
				if (!IsSet)
					throw new UnexpectedObjectException(
						msg != null ? msg : "cannot return an optional value when it's not set");
				
				return m_Value;
			}

			public V ExpectAs<V>(string msg = null) where V : T
			{
				try {
					return (V)Expect(msg);
				}
				catch(Exception e) {
					if(msg == null)
						msg = String.Format("object could not be given as {0}", typeof(V));

					throw new UnexpectedObjectException(msg, e);
				}
			}

			public T ExpectOr(T def)
			{
				if (!IsSet)
					return def;

				return m_Value;
			}
		}
	}
}
