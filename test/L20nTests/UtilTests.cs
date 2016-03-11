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
using System.IO;
using NUnit.Framework;

using L20n.Utils;
using L20n.Exceptions;

namespace L20nTests
{
	[TestFixture()]
	public class UtilTests
	{	
		[Test()]
		public void OptionalTests()
		{
			var a = new Optional<string>(null);
			Assert.IsFalse(a.IsSet);
			Assert.Throws<UnexpectedObjectException>(() => a.Expect());
			Assert.AreEqual("Oi", a.ExpectOr ("Oi"));

			var b = new Optional<string>("Hello, World!");
			Assert.IsTrue(b.IsSet);
			Assert.AreEqual("Hello, World!", b.Expect());
			Assert.AreEqual("Hello, World!", b.ExpectOr("Goodbye!"));
		}

		[Test()]
		public void ShadowStackTests()
		{
			var stack = new ShadowStack<int>();
			
			Assert.Throws<InvalidOperationException>(() => stack.Pop("oops"));

			stack.Push("apples", 1);
			stack.Push("bananas", 2);
			stack.Push("apples", 5);

			Assert.AreEqual(5, stack.Pop("apples"));
			Assert.AreEqual(2, stack.Pop("bananas"));
			
			stack.Push("bananas", 8);
			stack.Push("oranges", 99);
			stack.Push("apples", 34);
			
			Assert.AreEqual(8, stack.Pop("bananas"));
			Assert.AreEqual(34, stack.Pop("apples"));
			Assert.AreEqual(1, stack.Pop("apples"));
			Assert.AreEqual(99, stack.Pop("oranges"));

			Assert.Throws<InvalidOperationException>(() => stack.Pop ("apples"));
		}
	}
}
