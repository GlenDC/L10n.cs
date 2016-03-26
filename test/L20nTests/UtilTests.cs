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

using L20nCore.Utils;
using L20nCore.Exceptions;

namespace L20nCoreTests
{
	[TestFixture()]
	public class UtilTests
	{	
		[Test()]
		public void OptionalTests()
		{
			var a = new Option<string>();
			Assert.IsFalse(a.IsSet);
			Assert.Throws<UnexpectedObjectException>(() => a.Unwrap());
			Assert.AreEqual("Oi", a.UnwrapOr("Oi"));

			var b = new Option<string>("Hello, World!");
			Assert.IsTrue(b.IsSet);
			Assert.AreEqual("Hello, World!", b.Unwrap());
			Assert.AreEqual("Hello, World!", b.UnwrapOr("Goodbye!"));

			var c = new Option<int>(5);
			Assert.AreEqual(42, c.Map((x) => new Option<int>(37 + x)).Unwrap());
			
			var d = new Option<int>();
			Assert.IsFalse (d.Map((x) => new Option<int>(x)).IsSet);
			Assert.AreEqual(42, d.MapOr(42, (x) => x));
			Assert.AreEqual(42, d.MapOrElse((x) => x, () => c.MapOr(0, (x) => 37 + x)));

			Assert.IsFalse(a.And(b).IsSet);
			Assert.IsTrue(b.And(c).IsSet);
			Assert.AreEqual(5, b.And(c).Unwrap());
			
			Assert.IsTrue(a.Or(b).IsSet);
			Assert.AreEqual(b.Or(a), a.Or(b));
			Assert.AreEqual(5, d.Or(c).Unwrap());
			
			Assert.AreEqual(5, c.OrElse(() => new Option<int>(42)).Unwrap());
			Assert.AreEqual(42, d.OrElse(() => new Option<int>(42)).Unwrap());
		}

		[Test()]
		public void ShadowStackTests()
		{
			var stack = new ShadowStack<string>();
			
			Assert.Throws<InvalidOperationException>(() => stack.Pop("oops"));

			stack.Push("fruit", "banana");
			stack.Push("vegetable", "bean");
			stack.Push("meat", "steak");
			
			Assert.AreEqual("banana", stack.Peek("fruit"));
			Assert.AreEqual("bean", stack.Peek("vegetable"));

			Assert.AreEqual("bean", stack.Pop("vegetable"));
			Assert.AreEqual("banana", stack.Pop("fruit"));
			
			stack.Push("fruit", "apple");
			stack.Push("fruit", "orange");
			stack.Push("vegetable", "carrot");
			
			Assert.AreEqual("orange", stack.Pop("fruit"));
			Assert.AreEqual("apple", stack.Pop("fruit"));
			Assert.AreEqual("steak", stack.Pop("meat"));
			Assert.AreEqual("carrot", stack.Pop("vegetable"));

			Assert.Throws<InvalidOperationException>(() => stack.Pop("fruit"));
		}
	}
}
