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

using L20n.Objects;
using System.Collections;

namespace L20n
{
	namespace Utils
	{
		public sealed class ShadowStack<T>
		{
			private Dictionary<string, Stack<T>> m_StackDictionary;
			
			public ShadowStack()
			{
				m_StackDictionary = new Dictionary<string, Stack<T>>();
			}

			public void Push(string key, T value)
			{
				var stack = CreateOrGetStack(key);
				stack.Push(value);
			}
			
			public T Peek(string key)
			{
				var stack = CreateOrGetStack(key);
				return stack.Peek();
			}

			public T Pop(string key)
			{
				var stack = CreateOrGetStack(key);
				return stack.Pop();
			}
			
			private Stack<T> CreateOrGetStack(string key)
			{
				Stack<T> stack;
				if (!m_StackDictionary.TryGetValue(key, out stack)) {
					stack = new Stack<T>();
					m_StackDictionary.Add(key, stack);
				}

				return stack;
			}
		}
	}
}
