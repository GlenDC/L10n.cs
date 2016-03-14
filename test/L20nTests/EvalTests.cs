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

using L20n.IO;
using L20n.IO.Parsers;
using L20n.IO.Parsers.Expressions;
using L20n.Exceptions;
using System.Collections.Generic;

namespace L20nTests
{
	[TestFixture()]
	public class EvalTests
	{
		private L20n.Internal.LocaleContext m_DummyContext;

		public EvalTests()
		{
			m_DummyContext =
				(new L20n.Internal.LocaleContext.Builder())
					.Build(new Dictionary<string, L20n.Objects.GlobalValue>(), null);
		}

		[Test()]
		public void NonContextExpressionTests()
		{
			// Simple Primitives
			Assert.AreEqual(42,
				ParseAndEvalAs<L20n.Objects.Literal>("42").Value);
			Assert.AreEqual(-123,
				ParseAndEvalAs<L20n.Objects.Literal>("-123").Value);
			Assert.AreEqual("Hello, World!",
			    ParseAndEvalAs<L20n.Objects.StringOutput>("'Hello, World!'").Value);

			// Unary Expressions (I)
			Assert.AreEqual(-3,
				ParseAndEvalAs<L20n.Objects.Literal>("-(-(-3))").Value);
			Assert.AreEqual(3,
				ParseAndEvalAs<L20n.Objects.Literal>("+(+(-(-(+3))))").Value);

			// Binary Number Math Expressions
			Assert.AreEqual(-2,
				ParseAndEvalAs<L20n.Objects.Literal>("2 + 3 * 10 / 3 % 2 * 2 + 1 - 5").Value);
			Assert.AreEqual(-10,
				ParseAndEvalAs<L20n.Objects.Literal>("(-3 - 2) + -5").Value);
			Assert.AreEqual(-15,
				ParseAndEvalAs<L20n.Objects.Literal>("((10 / 2) * 3) * -1").Value);
			Assert.AreEqual(2,
				ParseAndEvalAs<L20n.Objects.Literal>("17 % 5").Value);

			// Binary Number Compare Expressions
			Assert.AreEqual(true,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("2 < 5").Value);
			Assert.AreEqual(false,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("3 < 3").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("3 <= 1 + 1 + 2").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("5 > 2").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("0 >= 0").Value);

			// Unary Expressions (II)
			Assert.AreEqual(true,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("!(3 < 3)").Value);
			Assert.AreEqual(false,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("!(3 >= 1)").Value);

			// Binary Compare Operations (works for strings, literals and booleans)
			Assert.AreEqual(false,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("(3 > 2) == (5 < 2)").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("(!(3 > 2)) == (3 < 1)").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("'hello' == 'hello'").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("1 + 2 == 4 - 3 + 1 + 1").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20n.Objects.BooleanValue>("5 != 16").Value);

			// Logical Expression(s)
			Assert.AreEqual(true,
				ParseAndEvalAs<L20n.Objects.BooleanValue>(
					"2 == 2 && 5 > 2").Value);
			Assert.AreEqual(true,
				ParseAndEvalAs<L20n.Objects.BooleanValue>(
					"2 > 8 || 10 == 5 + 5").Value);

			// Conditional Expression(s)
			Assert.AreEqual("John",
				ParseAndEvalAs<L20n.Objects.StringOutput>(
					"'English' != 'Dutch' ? 'John' : 'Jan'").Value);
		}

		private T ParseAndEvalAs<T>(string buffer)
			where T: L20n.Objects.L20nObject
		{
			return (T) Expression.Parse(NC(buffer)).Eval()
				.Eval(m_DummyContext).Unwrap();
		}

		private CharStream NC(string buffer)
		{
			return new CharStream(buffer);
		}
	}
}
