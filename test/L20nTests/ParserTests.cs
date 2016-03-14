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
	public class ParserTests
	{
		[Test()]
		public void WhiteSpaceTests()
		{
			var stream = NC("     ");

			// This will read everything
			WhiteSpace.Parse(stream, false);
			// This will not read anything, but it's optional
			// so it will not give an exception
			WhiteSpace.Parse(stream, true);
			// This will fail as it's not optional
			Assert.Throws<ParseException> (
				() => WhiteSpace.Parse(stream, false));
		}

		[Test()]
		public void CommentTests()
		{
			// Parsing an empty comment
			Comment.Parse(NC("/**/"));
			// Parsing a normal comment
			Comment.Parse(NC("/* A Comment */"));
			// Parsing a comment with a lot of stars
			Comment.Parse(NC("/********* Bling Bling **********/"));
			// This will fail as a comment was never found
			Assert.Throws<ParseException> (
				() => Comment.Parse(NC("No Comment */")));
			// This will fail as the comment was never terminated
			Assert.Throws<ParseException> (
				() => Comment.Parse(NC("Unfinished Comment */")));

			// PeekAndParse can be used to make sure that
			// you only try to parse something that at least looks like a comment
			Assert.IsTrue(Comment.PeekAndParse(NC("/* A Comment */")));
			// When the start doesn't look like a comment it will simply return false
			Assert.IsFalse(Comment.PeekAndParse(NC("Hello")));
			// It will still throw an assert however,
			// if the start looks like a comment, but it turned out to be a trap
			Assert.Throws<ParseException> (
				() => Comment.PeekAndParse(NC("/* What can go wrong")));

			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<ParseException>(() => Comment.Parse(NC("")));
		}

		[Test()]
		public void QuoteTests()
		{
			// Single quotes can be used for both single and multi-line purposes
			Assert.AreEqual("'", Quote.Parse(NC("'")).ToString());
			Assert.AreEqual("'''", Quote.Parse(NC("'''")).ToString());
			// Same goes for double quotes
			Assert.AreEqual("\"", Quote.Parse(NC("\"")).ToString());
			Assert.AreEqual("\"\"\"", Quote.Parse(NC("\"\"\"")).ToString());
			// If > 4 quotes are given, we still won't read more than 3 (multi-line)
			Assert.AreEqual("'''", Quote.Parse(NC("''''")).ToString());
			// If 2 quotes are given, we will just read 1 (single-line)
			Assert.AreEqual("'", Quote.Parse(NC("''")).ToString());

			// You can also require a certain quote, in case you require a matching pair
			var s = NC("''''");
			var quote = Quote.Parse(s);
			// will fail now because we try to match a multi-line
			// with a single-line quote
			Assert.Throws<ParseException> (
				() => Quote.Parse (s, quote));
			
			// Single Quotes can't be matched with Double Quotes
			s = NC("'\""); quote = Quote.Parse(s);
			Assert.Throws<ParseException> (
				() => Quote.Parse (s, quote));
			
			// Matching pairs should give you a good night sleep
			s = NC("''"); quote = Quote.Parse(s);
			Assert.AreEqual(
				quote.ToString(),
				Quote.Parse(s, quote).ToString());
			
			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<ParseException>(() => Quote.Parse(NC("")));
		}

		private delegate string StringToString(string str, bool allow_underscore);

		[Test()]
		public void RawIdentifierTests()
		{
			StringToString strToStr = (string str, bool allow_underscore) =>
				Identifier.Parse(NC(str), allow_underscore);

			// an identifier is a string that only contains word-characters
			Assert.AreEqual("aBcDeFgH", strToStr("aBcDeFgH", true));
			Assert.AreEqual("Hello_World", strToStr("Hello_World", true));
			Assert.AreEqual("_private", strToStr("_private", true));

			// often we won't allow identifiers starting with a hash
			Assert.Throws<ParseException>(() => strToStr("_shit", false));

			// white-spaces are not included in that
			Assert.AreEqual("Hello", strToStr("Hello", true));
			// neither or dashes
			Assert.AreEqual("glen", strToStr("glen-dc", true));
			// starting with a non-word char will however make it fail
			Assert.Throws<ParseException>(() => Identifier.Parse(NC(" oh"), false));

			// You can also Parse identifiers in a fail-safe way
			string id;
			Assert.IsTrue(Identifier.PeekAndParse(NC("Ho_Ho_Ho"), out id, false));
			Assert.AreEqual("Ho_Ho_Ho", id);
			Assert.IsFalse(Identifier.PeekAndParse(NC(" fails"), out id, false));

			// passing in an EOF stream will give an <EOF> ParseException
			Assert.Throws<ParseException>(() => Identifier.Parse(NC(""), false));
		}

		private delegate int StringToNumber(string s);

		[Test()]
		public void LiteralTests()
		{
			StringToNumber stringToNumber = (String str) =>
				((L20n.Objects.Literal)Literal.Parse(NC(str)).Eval()).Value;

			// any integer is a valid literal
			Assert.AreEqual(-123, stringToNumber("-123"));
			Assert.AreEqual(42, stringToNumber("+42"));
			Assert.AreEqual(7, stringToNumber("7"));

			// decimals will be ignored and make for
			// an invalid buffer later on
			Assert.AreEqual(5, stringToNumber("5.2"));
			Assert.Throws<ParseException>(() => Literal.Parse(NC(".2")));
			
			// passing in an EOF stream will give an <EOF> ParseException
			Assert.Throws<ParseException>(() => Literal.Parse(NC("")));
		}

		[Test()]
		public void HashValueTests()
		{
			// hash tables can be pretty easy
			Primary.Parse (NC ("{hello:'world'}"));

			// hash tables cannot be empty though
			Assert.Throws<ParseException>(() => HashValue.Parse(NC("{}")));

			// hash tables can also have a default
			Primary.Parse (NC ("{*what:'ok', yes: 'no'}"));
			
			// hash tables cannot contain duplicate identifiers
			Assert.Throws<ParseException>(() => Primary.Parse(NC("{a: 'a', a: 'b'}")));
			// multiple defaults are also not allowed
			Assert.Throws<ParseException>(() => Primary.Parse(NC("{*a: 'a', *b: 'b'}")));
			
			// hash tables can also be nested
			Primary.Parse(NC (@"{
				short: {
				  	*subjective: 'Loki',
				    objective: 'Loki',
				    possessive: 'Lokis',
				},
				long: 'Loki Mobile Client'
			}"));
			
			// passing in an EOF stream will give an <EOF> ParseException
			Assert.Throws<ParseException>(() => HashValue.Parse(NC("")));
		}

		[Test()]
		public void IdentifierExpressionTests()
		{
			// One identifier parser to rule them all (4)
			TypeAssert<L20n.IO.AST.Identifier>(
				IdentifierExpression.Parse(NC("identifier")));
			TypeAssert<L20n.IO.AST.Variable>(
				IdentifierExpression.Parse(NC("$normal_variable")));
			TypeAssert<L20n.IO.AST.Global>(
				IdentifierExpression.Parse(NC("@global_variable")));

			// Anything that would fail the RawIdentifier tests
			// would also fail this one, as it wraps around
			Assert.Throws<ParseException>(
				() => IdentifierExpression.Parse(NC(" no_prefix_space_allowed")));
			Assert.Throws<ParseException>(
				() => IdentifierExpression.Parse(NC("-only_underscores_and_letters_are_allowed")));
			
			// passing in an EOF stream will give an <EOF> ParseException
			Assert.Throws<ParseException>(() => IdentifierExpression.Parse(NC("")));
		}
		
		[Test()]
		public void PrimaryExpressionTests()
		{
			// One primary expression to rule them all

			// literals
			TypeAssert<L20n.IO.AST.Literal>(
				Primary.Parse(NC("-42")));
			TypeAssert<L20n.IO.AST.Literal>(
				Primary.Parse(NC("+10")));
			TypeAssert<L20n.IO.AST.Literal>(
				Primary.Parse(NC("123")));

			// string values
			TypeAssert<L20n.IO.AST.StringValue>(
				Primary.Parse(NC("\"Hello Dude!\"")));
			TypeAssert<L20n.IO.AST.StringValue>(
				Primary.Parse(NC("'this works as well'")));
			TypeAssert<L20n.IO.AST.StringValue>(
				Primary.Parse(NC("'Hello {{ foo.bar }}'")));
			TypeAssert<L20n.IO.AST.StringValue>(
				Primary.Parse(NC("'Hello {{ $person.name }}'")));
			TypeAssert<L20n.IO.AST.StringValue>(
				Primary.Parse(NC("'It is {{ @time.hour }} o\' clock.'")));

			// hash values
			TypeAssert<L20n.IO.AST.HashValue>(
				Primary.Parse(NC("{hello:'world'}")));
			TypeAssert<L20n.IO.AST.HashValue>(
				Primary.Parse(NC("{dev: 'go develop' , debug: 'go debug',}")));
			TypeAssert<L20n.IO.AST.HashValue>(
				Primary.Parse(NC("{one:{a:'a',b:'b'},two:'2', three:'3'}")));

			// identifier expressions
			TypeAssert<L20n.IO.AST.Variable>(
				Primary.Parse(NC("$ola")));
			TypeAssert<L20n.IO.AST.Global>(
				Primary.Parse(NC("@hello")));
			TypeAssert<L20n.IO.AST.Identifier>(
				Primary.Parse(NC("bom_dia")));

			// all other type of input should fail
			Assert.Throws<ParseException>(() => Primary.Parse(NC("     ")));
			Assert.Throws<ParseException>(() => Primary.Parse(NC("<hello 'world'>")));
			
			// passing in an EOF stream will give an <EOF> ParseException
			Assert.Throws<ParseException>(() => Primary.Parse(NC("")));
		}

		[Test()]
		public void ExpressionParseTests()
		{
			// this is a simple test to see if we can actually parse all the different expressions
			// this test does NOT guarantee that they also evaluate correctly

			// Primary/Identifier Expressions
			ExpressionParseTest<L20n.Objects.Variable>
				("$id");
			ExpressionParseTest<L20n.Objects.Global>
				("@id");
			ExpressionParseTest<L20n.Objects.Identifier>
				("whatever");
			ExpressionParseTest<L20n.Objects.Literal>
				("42");
			ExpressionParseTest<L20n.Objects.StringValue>
				("'Hello, World!'");
			ExpressionParseTest<L20n.Objects.HashValue>
				("{age: '23', location: 'unknown'}");
			
			// Parenthesis Expressions
			ExpressionParseTest<L20n.Objects.Literal>
				("42");
			ExpressionParseTest<L20n.Objects.Identifier>
				("whatever");
			ExpressionParseTest<L20n.Objects.Literal>
				("(42)");
			ExpressionParseTest<L20n.Objects.Variable>
				("((($OK)))");
			ExpressionParseTest<L20n.Objects.Literal>
				("((((((((((((((5))))))))))))))");
			
			// Property Expressions
			ExpressionParseTest<L20n.Objects.PropertyExpression>
				("hello.world");
			ExpressionParseTest<L20n.Objects.PropertyExpression>
				("one.two.three");
			
			// Call Expressions
			ExpressionParseTest<L20n.Objects.CallExpression>
				("hello(world)");
			ExpressionParseTest<L20n.Objects.CallExpression>
				("hello(bonjour(oi(world)))");
			ExpressionParseTest<L20n.Objects.CallExpression>
				("greetings('hello', 'oi', 'bom dia', 'bonjour')");
			ExpressionParseTest<L20n.Objects.CallExpression>
				("echo('hello world', 5)");
			
			// Unary Expressions
			ExpressionParseTest<L20n.Objects.NegativeExpression>
				("-42");
			ExpressionParseTest<L20n.Objects.PositiveExpression>
				("+42");
			ExpressionParseTest<L20n.Objects.NegateExpression>
				("!true");
			ExpressionParseTest<L20n.Objects.NegateExpression>
				("!(!(!(!(!$what))))");
			
			// Binary Expressions
			ExpressionParseTest<L20n.Objects.SubstractExpression>
				("-52-10");
			ExpressionParseTest<L20n.Objects.AddExpression>
				("(32+10)");
			ExpressionParseTest<L20n.Objects.MultiplyExpression>
				("(21*2)");
			ExpressionParseTest<L20n.Objects.ModuloExpression>
				("102%60");
			ExpressionParseTest<L20n.Objects.ModuloExpression>
				("102%(30*2)");
			ExpressionParseTest<L20n.Objects.AddExpression>
				("1+2+3+4+5");
			ExpressionParseTest<L20n.Objects.GreaterThanExpression>
				("10 > 2");
			ExpressionParseTest<L20n.Objects.GreaterThanOrEqualExpression>
				("10 >= 2");
			ExpressionParseTest<L20n.Objects.LessThanExpression>
				("10 < 5 * 100");
			ExpressionParseTest<L20n.Objects.LessThanOrEqualExpression>
				("10 <= 5 + 8 - 3");
			ExpressionParseTest<L20n.Objects.IsEqualExpression> // parenthesis are overrated
				("5 + 5 == 3 * 2 + 4");
			ExpressionParseTest<L20n.Objects.IsNotEqualExpression>
				("41 != answer");
			
			// Logical Expressions
			ExpressionParseTest<L20n.Objects.AndExpression>
				("0 && 1 && 2");
			ExpressionParseTest<L20n.Objects.OrExpression>
				("false || true");
			ExpressionParseTest<L20n.Objects.OrExpression>
				("false || false || false || 42");
			
			// Logical Expressions
			ExpressionParseTest<L20n.Objects.IfElseExpression>
				("true ? 42 : 41");
			ExpressionParseTest<L20n.Objects.IfElseExpression>
				("true ? shit : 'I would rather want this'");
			ExpressionParseTest<L20n.Objects.IfElseExpression>
				("true || false ? (ok) : 42");
		}

		[Test()]
		public void InvalidEntryParseTests()
		{
			var builder = new L20n.Internal.LocaleContext.Builder();
			// invalid comment examples
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("/* non-closed comment"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("non-opened comment*/"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("/* almost a correct comment *"), builder));

			// invalid import examples
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("import 'nope'"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("import ('nope')"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("import('nope'"), builder));
			
			// invalid macro examples
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<foo () {'no space allowed before first parenthesis'}>"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<foo() {'invalid expression}>"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<foo(nope) {'non-variable as identifier'}>"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<foo($no_expression_defined) {}>"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<foo() {'closing curly brace missing'>"), builder));

			// invalid entity examples
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<'no identifier given'>"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<onlyAnIdentifierGiven>"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<no'space'>"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<invalid('index') 'should be enclosed by [], not ()'>"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<invalid ['index'] 'no space allowed before the first ['>"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<invalid['value'] 42>"), builder));
		}


		private void ExpressionParseTest<T>(string input) {
			var stream = new CharStream (input);
			TypeAssert<T>(Expression.Parse(stream).Eval());
			if (stream.InputLeft ())
				throw new ParseException("stream is non-empty: " + stream.ReadUntilEnd());
		}

		private void TypeAssert<T>(object obj)
		{
			Assert.AreEqual(typeof(T), obj.GetType());
		}

		private CharStream NC(string buffer)
		{
			return new CharStream(buffer);
		}
	}
}
