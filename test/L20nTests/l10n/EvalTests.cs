// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

using L20nCore.L10n.IO.Parsers;
using L20nCore.L10n.IO.Parsers.Expressions;

namespace L20nCoreTests
{
	namespace L10n
	{
		[TestFixture()]
		/// <summary>
		/// Tests to check if our expressions evaluate to an expected value.
		/// </summary>
		public class EvalTests
		{
			private L20nCore.L10n.Internal.LocaleContext m_DummyContext;

			public EvalTests()
			{
				m_DummyContext =
					(new L20nCore.L10n.Internal.LocaleContext.Builder())
						.Build(new Dictionary<string, L20nCore.L10n.Objects.L10nObject>(), null);
			}

			[Test()]
			public void NonContextExpressionTests()
			{
				// Simple Primitives
				Assert.AreEqual(42,
					ParseAndEvalAs<L20nCore.L10n.Objects.Literal>("42").Value);
				Assert.AreEqual(-123,
				    ParseAndEvalAs<L20nCore.L10n.Objects.Literal>("-123").Value);
				Assert.AreEqual("Hello, World!",
				    ParseAndEvalAs<L20nCore.L10n.Objects.StringOutput>("'Hello, World!'").Value);

				// Unary Expressions (I)
				Assert.AreEqual(-3,
					ParseAndEvalAs<L20nCore.L10n.Objects.Literal>("-(-(-3))").Value);
				Assert.AreEqual(3,
					ParseAndEvalAs<L20nCore.L10n.Objects.Literal>("+(+(-(-(+3))))").Value);

				// Binary Number Math Expressions
				Assert.AreEqual(-2,
					ParseAndEvalAs<L20nCore.L10n.Objects.Literal>("2 + 3 * 10 / 3 % 2 * 2 + 1 - 5").Value);
				Assert.AreEqual(-10,
					ParseAndEvalAs<L20nCore.L10n.Objects.Literal>("(-3 - 2) + -5").Value);
				Assert.AreEqual(-15,
					ParseAndEvalAs<L20nCore.L10n.Objects.Literal>("((10 / 2) * 3) * -1").Value);
				Assert.AreEqual(2,
					ParseAndEvalAs<L20nCore.L10n.Objects.Literal>("17 % 5").Value);

				// Binary Number Compare Expressions
				Assert.AreEqual(true,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("2 < 5").Value);
				Assert.AreEqual(false,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("3 < 3").Value);
				Assert.AreEqual(true,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("3 <= 1 + 1 + 2").Value);
				Assert.AreEqual(true,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("5 > 2").Value);
				Assert.AreEqual(true,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("0 >= 0").Value);

				// Unary Expressions (II)
				Assert.AreEqual(true,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("!(3 < 3)").Value);
				Assert.AreEqual(false,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("!(3 >= 1)").Value);

				// Binary Compare Operations (works for strings, literals and booleans)
				Assert.AreEqual(false,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("(3 > 2) == (5 < 2)").Value);
				Assert.AreEqual(true,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("(!(3 > 2)) == (3 < 1)").Value);
				Assert.AreEqual(true,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("'hello' == 'hello'").Value);
				Assert.AreEqual(true,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("1 + 2 == 4 - 3 + 1 + 1").Value);
				Assert.AreEqual(true,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>("5 != 16").Value);

				// Logical Expression(s)
				Assert.AreEqual(true,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>(
						"2 == 2 && 5 > 2").Value);
				Assert.AreEqual(true,
					ParseAndEvalAs<L20nCore.L10n.Objects.BooleanValue>(
						"2 > 8 || 10 == 5 + 5").Value);

				// Conditional Expression(s)
				Assert.AreEqual("John",
					ParseAndEvalAs<L20nCore.L10n.Objects.StringOutput>(
						"'English' != 'Dutch' ? 'John' : 'Jan'").Value);
			}

			private T ParseAndEvalAs<T>(string buffer)
				where T: L20nCore.L10n.Objects.L10nObject
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
}
