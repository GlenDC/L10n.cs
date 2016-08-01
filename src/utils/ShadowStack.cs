// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

			private Dictionary<string, Stack<T>> m_StackDictionary;
            
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
