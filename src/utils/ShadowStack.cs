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

using L20nCore.Objects;
using System.Collections;

namespace L20nCore
{
	namespace Utils
	{
		/// <summary>
		/// A utility class that functions as a language stack that allows shadowing.
		/// This means that when we Push (Add) a value, we can add it on top of the already set value.
		/// When we Pop (Remove) a value, the last set value will be removed and returned.
		/// </summary>
		public sealed class ShadowStack<T>
            where T: class
		{
			private Dictionary<string, Stack<T>> m_StackDictionary;
            
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Utils.ShadowStack`1"/> class.
			/// </summary>
			public ShadowStack()
			{
				m_StackDictionary = new Dictionary<string, Stack<T>>();
			}

			/// <summary>
			/// Push the given <c>value</c> to the stack linked with <c>key</c>.
			/// </summary>
			public void Push(string key, T value)
			{
				var stack = CreateOrGetStack(key);
				stack.Push(value);
			}
            
			/// <summary>
			/// Returns a reference to the last set value on the stack linked with <c>key</c>.
			/// Throws an exception in case the linked stack is empty.
			/// </summary>
			/// <param name="key">Key.</param>
			public T Peek(string key)
			{
				var stack = CreateOrGetStack(key);
				return stack.Peek();
			}

			/// <summary>
			/// Returns a reference to the last set value on the stack linked with <c>key</c>.
			/// <c>null</c> gets returned in case the linked stack is empty.
			/// </summary>
			public T PeekSafe(string key)
			{
				var stack = CreateOrGetStack(key);
				if (stack.Count > 0)
					return stack.Peek();

				return null;
			}

			/// <summary>
			/// Removes and returns the last set value from the stack linked with <c>key</c>.
			/// Throws an exception in case the linked stack is empty.
			/// </summary>
			public T Pop(string key)
			{
				var stack = CreateOrGetStack(key);
				return stack.Pop();
			}
            
			/// <summary>
			/// Removes and returns the last set value from the stack linked with <c>key</c>.
			/// <c>null</c> gets returned in case the linked stack is empty.
			/// </summary>
			public T PopSafe(string key)
			{
				var stack = CreateOrGetStack(key);
				if (stack.Count > 0)
					return stack.Pop();

				return null;
			}
            
			private Stack<T> CreateOrGetStack(string key)
			{
				Stack<T> stack;
				if (!m_StackDictionary.TryGetValue(key, out stack))
				{
					stack = new Stack<T>();
					m_StackDictionary.Add(key, stack);
				}

				return stack;
			}
		}
	}
}
