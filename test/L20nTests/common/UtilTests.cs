// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using NUnit.Framework;

using L20nCore.Common.Utils;
using L20nCore.Common.Exceptions;

namespace L20nCoreTests
{
	namespace Common
	{
		[TestFixture()]
		/// <summary>
		/// Tests for some of the utility classes of this project.
		/// </summary>
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
				Assert.IsFalse(d.Map((x) => new Option<int>(x)).IsSet);
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
}
