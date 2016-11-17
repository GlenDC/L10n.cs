// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using NUnit.Framework;

using L20nCore.IO;
using L20nCore.IO.Parsers.L10n;
using L20nCore.IO.Parsers.L10n.Expressions;
using L20nCore.Exceptions;
using System.Collections.Generic;

namespace L20nCoreTests
{
	[TestFixture()]
	/// <summary>
	/// Tests to check if our expressions evaluate to an expected value.
	/// </summary>
	public class L10nEvalTests
	{
		private L20nCore.Internal.LocaleContext m_DummyContext;

		public L10nEvalTests()
		{
			m_DummyContext =
				(new L20nCore.Internal.LocaleContext.L10nBuilder())
					.Build(new Dictionary<string, L20nCore.Objects.L20nObject>(), null);
		}

		[Test()]
		public void NonContextExpressionTests()
		{
			// Simple Primitives
			Assert.AreEqual(42,
				ParseAndEvalAs<L20nCore.Objects.Literal>("42").Value);
			Assert.AreEqual(-123,
				ParseAndEvalAs<L20nCore.Objects.Literal>("-123").Value);
			Assert.AreEqual("Hello, World!",
			    ParseAndEvalAs<L20nCore.Objects.StringOutput>("'Hello, World!'").Value);

			// Unary Expressions (I)
			Assert.AreEqual(-3,
				ParseAndEvalAs<L20nCore.Objects.Literal>("-(-(-3))").Value);
			Assert.AreEqual(3,
				ParseAndEvalAs<L20nCore.Objects.Literal>("+(+(-(-(+3))))").Value);

			// Binary Number Math Expressions
			Assert.AreEqual(-2,
				ParseAndEvalAs<L20nCore.Objects.Literal>("2 + 3 * 10 / 3 % 2 * 2 + 1 - 5").Value);
			Assert.AreEqual(-10,
				ParseAndEvalAs<L20nCore.Objects.Literal>("(-3 - 2) + -5").Value);
			Assert.AreEqual(-15,
				ParseAndEvalAs<L20nCore.Objects.Literal>("((10 / 2) * 3) * -1").Value);
			Assert.AreEqual(2,
				ParseAndEvalAs<L20nCore.Objects.Literal>("17 % 5").Value);

			// Binary Number Compare Expressions
			Assert.AreEqual(true,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("2 < 5").Value);
			Assert.AreEqual(false,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("3 < 3").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("3 <= 1 + 1 + 2").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("5 > 2").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("0 >= 0").Value);

			// Unary Expressions (II)
			Assert.AreEqual(true,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("!(3 < 3)").Value);
			Assert.AreEqual(false,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("!(3 >= 1)").Value);

			// Binary Compare Operations (works for strings, literals and booleans)
			Assert.AreEqual(false,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("(3 > 2) == (5 < 2)").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("(!(3 > 2)) == (3 < 1)").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("'hello' == 'hello'").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("1 + 2 == 4 - 3 + 1 + 1").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>("5 != 16").Value);

			// Logical Expression(s)
			Assert.AreEqual(true,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>(
					"2 == 2 && 5 > 2").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20nCore.Objects.BooleanValue>(
					"2 > 8 || 10 == 5 + 5").Value);

			// Conditional Expression(s)
			Assert.AreEqual("John",
				ParseAndEvalAs<L20nCore.Objects.StringOutput>(
					"'English' != 'Dutch' ? 'John' : 'Jan'").Value);
		}

		private T ParseAndEvalAs<T>(string buffer)
			where T: L20nCore.Objects.L20nObject
		{
			return (T)Expression.Parse(NC(buffer)).Eval()
				.Eval(m_DummyContext);
		}

		private CharStream NC(string buffer)
		{
			return new CharStream(buffer);
		}
	}
}
