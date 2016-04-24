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
using System.IO;
using NUnit.Framework;

using L20nCore.IO;
using L20nCore.IO.Parsers;
using L20nCore.IO.Parsers.Expressions;
using L20nCore.Exceptions;
using System.Collections.Generic;

namespace L20nCoreTests
{
	[TestFixture()]
	/// <summary>
	/// Tests for all the individual parsers.
	/// The l20n parser is a parser of parsers, so if all the small pieces work,
	/// there is a very high chance that the final result is OK.
	/// </summary>
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
			Assert.Throws<ParseException>(
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
			Assert.Throws<ParseException>(
				() => Comment.Parse(NC("No Comment */")));
			// This will fail as the comment was never terminated
			Assert.Throws<ParseException>(
				() => Comment.Parse(NC("Unfinished Comment */")));

			// PeekAndParse can be used to make sure that
			// you only try to parse something that at least looks like a comment
			Assert.IsTrue(Comment.PeekAndParse(NC("/* A Comment */")));
			// When the start doesn't look like a comment it will simply return false
			Assert.IsFalse(Comment.PeekAndParse(NC("Hello")));
			// It will still throw an assert however,
			// if the start looks like a comment, but it turned out to be a trap
			Assert.Throws<ParseException>(
				() => Comment.PeekAndParse(NC("/* What can go wrong")));

			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<ParseException>(() => Comment.Parse(NC("")));
		}

		[Test()]
		public void QuoteTests()
		{
			Assert.AreEqual("'", Quote.Parse(NC("'")).ToString());
			Assert.AreEqual("\"", Quote.Parse(NC("\"")).ToString());
			// If 2 quotes are given, we will just read 1 (single-line)
			Assert.AreEqual("'", Quote.Parse(NC("''")).ToString());

			// You can also require a certain quote, in case you require a matching pair
			var s = NC("''''");
			var quote = Quote.Parse(s);
			
			// Single Quotes can't be matched with Double Quotes
			s = NC("'\"");
			quote = Quote.Parse(s);
			Assert.Throws<ParseException>(
				() => Quote.Parse(s, quote));
			
			// Matching pairs should give you a good night sleep
			s = NC("''");
			quote = Quote.Parse(s);
			Assert.AreEqual(
				quote.ToString(),
				Quote.Parse(s, quote).ToString());
			
			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<ParseException>(() => Quote.Parse(NC("")));
		}

		private delegate string StringToString(string str,bool allow_underscore);

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
				((L20nCore.Objects.Literal)Literal.Parse(NC(str)).Eval()).Value;

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
			Primary.Parse(NC("{hello:'world'}"));

			// hash tables cannot be empty though
			Assert.Throws<ParseException>(() => HashValue.Parse(NC("{}")));

			// hash tables can also have a default
			Primary.Parse(NC("{*what:'ok', yes: 'no'}"));
			
			// hash tables cannot contain duplicate identifiers
			Assert.Throws<ParseException>(() => Primary.Parse(NC("{a: 'a', a: 'b'}")));
			// multiple defaults are also not allowed
			Assert.Throws<ParseException>(() => Primary.Parse(NC("{*a: 'a', *b: 'b'}")));
			
			// hash tables can also be nested
			Primary.Parse(NC(@"{
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
			TypeAssert<L20nCore.IO.AST.Identifier>(
				IdentifierExpression.Parse(NC("identifier")));
			TypeAssert<L20nCore.IO.AST.Variable>(
				IdentifierExpression.Parse(NC("$normal_variable")));
			TypeAssert<L20nCore.IO.AST.Global>(
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
			TypeAssert<L20nCore.IO.AST.Literal>(
				Primary.Parse(NC("-42")));
			TypeAssert<L20nCore.IO.AST.Literal>(
				Primary.Parse(NC("+10")));
			TypeAssert<L20nCore.IO.AST.Literal>(
				Primary.Parse(NC("123")));

			// string values
			TypeAssert<L20nCore.IO.AST.StringValue>(
				Primary.Parse(NC("\"Hello Dude!\"")));
			TypeAssert<L20nCore.IO.AST.StringValue>(
				Primary.Parse(NC("'this works as well'")));
			TypeAssert<L20nCore.IO.AST.StringValue>(
				Primary.Parse(NC("'Hello {{ foo.bar }}'")));
			TypeAssert<L20nCore.IO.AST.StringValue>(
				Primary.Parse(NC("'Hello {{ $person.name }}'")));
			TypeAssert<L20nCore.IO.AST.StringValue>(
				Primary.Parse(NC("'@hour {{ _hours[plural(@hour)] }}'")));
			TypeAssert<L20nCore.IO.AST.StringValue>(
				Primary.Parse(NC("'It is {{ @time.hour }} o\' clock.'")));

			// hash values
			TypeAssert<L20nCore.IO.AST.HashValue>(
				Primary.Parse(NC("{hello:'world'}")));
			TypeAssert<L20nCore.IO.AST.HashValue>(
				Primary.Parse(NC("{dev: 'go develop' , debug: 'go debug',}")));
			TypeAssert<L20nCore.IO.AST.HashValue>(
				Primary.Parse(NC("{one:{a:'a',b:'b'},two:'2', three:'3'}")));

			// identifier expressions
			TypeAssert<L20nCore.IO.AST.Variable>(
				Primary.Parse(NC("$ola")));
			TypeAssert<L20nCore.IO.AST.Global>(
				Primary.Parse(NC("@hello")));
			TypeAssert<L20nCore.IO.AST.Identifier>(
				Primary.Parse(NC("bom_dia")));

			// all other type of input should fail
			Assert.Throws<ParseException>(() => Primary.Parse(NC("     ")));
			Assert.Throws<ParseException>(() => Primary.Parse(NC("<hello 'world'>")));
			
			// passing in an EOF stream will give an <EOF> ParseException
			Assert.Throws<ParseException>(() => Primary.Parse(NC("")));
		}

		[Test()]
		public void OptimizeTests()
		{
			// String Values
			ExpressionParseTest(
				"5 + 2 =? something - 1",
				"'{{ 5 }} + {{ 2 }} =? {{ '{{ 'something' }} - {{ 1 }}' }}'");
			ExpressionParseTest<L20nCore.Objects.StringValue>
				("'{{ 5 }} + {{ 2 }} =? {{ x }}'");

			// Conditional Values
			ExpressionParseTest(5, "1 > 2 ? 2 : 5");
			ExpressionParseTest<L20nCore.Objects.IfElseExpression>
				("@x ? 2 : 5");

			// Binary Expression Values
			ExpressionParseTest(false, "2 == 3");
			ExpressionParseTest(true, "'hello' == 'hello'");
			ExpressionParseTest(true, "'bianca' != 'brianca'");
			ExpressionParseTest(true, "(2 == 2) != (3 == 2)");
			ExpressionParseTest(42, "37 + 5");
			ExpressionParseTest(-2, "(1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9) * 0 + 3 - 5");

			// Unary Expression Tests
			ExpressionParseTest(5, "--5");
			ExpressionParseTest(42, "-+-+-+-+-+-+-+-+-+-+-+-+42");
			ExpressionParseTest(true, "!!!!!!!!!!!!!!!(1 > 3)");
		}

		[Test()]
		public void ExpressionParseTests()
		{
			// this is a simple test to see if we can actually parse all the different expressions
			// this test does NOT guarantee that they also evaluate correctly

			// Primary/Identifier Expressions
			ExpressionParseTest<L20nCore.Objects.Variable>
				("$id");
			ExpressionParseTest<L20nCore.Objects.Global>
				("@id");
			ExpressionParseTest<L20nCore.Objects.Identifier>
				("whatever");
			ExpressionParseTest<L20nCore.Objects.Literal>
				("42");
			ExpressionParseTest<L20nCore.Objects.StringValue>
				("'Hello {{ user }}!'");
			ExpressionParseTest<L20nCore.Objects.StringOutput>
				("'Hello, World!'");
			ExpressionParseTest<L20nCore.Objects.HashValue>
				("{age: '23', location: 'unknown'}");

			// just some quick string value tests
			ExpressionParseTest("Glen's bottle.", @"'Glen\'s bottle.'");
			ExpressionParseTest(((char)Convert.ToInt32("F6222", 16)).ToString(), @"'\u0F6222'");
			ExpressionParseTest(((char)Convert.ToInt32("F6222", 16)).ToString(), "'U+0F6222'");
			ExpressionParseTest("{0} 1", "'{0} {{ 1 }}'");
			ExpressionParseTest("{{ 0 }} 1", @"'\{\{ 0 \}\} {{ 1 }}'");
			ExpressionParseTest("Whatever Forever!", "'   Whatever    Forever!  '");
			ExpressionParseTest("Whatever Forever, Bro! Give me the 7!", @"'   
															Whatever
								Forever, Bro!

					Give me the 7!  
								'");
			ExpressionParseTest("天长地久", @"'   
															天长
								地久
								'");

			// string expander tests
			ExpressionParseTest<L20nCore.Objects.StringValue>
				("\"{{ third_person[@user.gender] }} said: '{{ temperature_desc(@temperature) }}'\"");
			
			// Parenthesis Expressions
			ExpressionParseTest<L20nCore.Objects.Literal>
				("42");
			ExpressionParseTest<L20nCore.Objects.Identifier>
				("whatever");
			ExpressionParseTest<L20nCore.Objects.Literal>
				("(42)");
			ExpressionParseTest<L20nCore.Objects.Variable>
				("((($OK)))");
			ExpressionParseTest<L20nCore.Objects.Literal>
				("((((((((((((((5))))))))))))))");
			
			// Property Expressions
			ExpressionParseTest<L20nCore.Objects.PropertyExpression>
				("hello.world");
			ExpressionParseTest<L20nCore.Objects.PropertyExpression>
				("one.two.three");
			ExpressionParseTest<L20nCore.Objects.PropertyExpression>
				("one[two].three[four].five");
			ExpressionParseTest<L20nCore.Objects.PropertyExpression>
				("one[@two.three]");
			
			// Call Expressions
			ExpressionParseTest<L20nCore.Objects.CallExpression>
				("hello(world)");
			ExpressionParseTest<L20nCore.Objects.CallExpression>
				("hello(bonjour(oi(world)))");
			ExpressionParseTest<L20nCore.Objects.CallExpression>
				("greetings('hello', 'oi', 'bom dia', 'bonjour')");
			ExpressionParseTest<L20nCore.Objects.CallExpression>
				("echo('hello world', 5)");
			
			// Unary Expressions
			ExpressionParseTest<L20nCore.Objects.NegativeExpression>
				("-x");
			ExpressionParseTest<L20nCore.Objects.PositiveExpression>
				("+x");
			ExpressionParseTest<L20nCore.Objects.NegateExpression>
				("!x");
			ExpressionParseTest<L20nCore.Objects.NegateExpression>
				("!(!(!(!(!$what))))");
			
			// Binary Expressions
			ExpressionParseTest<L20nCore.Objects.SubstractExpression>
				("-x-10");
			ExpressionParseTest<L20nCore.Objects.AddExpression>
				("(y+10)");
			ExpressionParseTest<L20nCore.Objects.MultiplyExpression>
				("(x*2)");
			ExpressionParseTest<L20nCore.Objects.ModuloExpression>
				("y%60");
			ExpressionParseTest<L20nCore.Objects.ModuloExpression>
				("x%(30*2)");
			ExpressionParseTest<L20nCore.Objects.AddExpression>
				("1+2+x+4+5");
			ExpressionParseTest<L20nCore.Objects.GreaterThanExpression>
				("x > 2");
			ExpressionParseTest<L20nCore.Objects.GreaterThanOrEqualExpression>
				("x >= 2");
			ExpressionParseTest<L20nCore.Objects.LessThanExpression>
				("x < 5 * 100");
			ExpressionParseTest<L20nCore.Objects.LessThanOrEqualExpression>
				("x <= 5 + 8 - 3");
			ExpressionParseTest<L20nCore.Objects.IsEqualExpression> // parenthesis are overrated
				("x + 5 == 3 * 2 + 4");
			ExpressionParseTest<L20nCore.Objects.IsNotEqualExpression>
				("41 != answer");
			
			// Logical Expressions
			ExpressionParseTest<L20nCore.Objects.AndExpression>
				("true && y && false");
			ExpressionParseTest<L20nCore.Objects.OrExpression>
				("false || x");
			ExpressionParseTest<L20nCore.Objects.OrExpression>
				("false || false || false || x");
			
			// Logical Expressions
			ExpressionParseTest<L20nCore.Objects.IfElseExpression>
				("y ? 42 : 41");
			ExpressionParseTest<L20nCore.Objects.IfElseExpression>
				("x ? shit : 'I would rather want this'");
			ExpressionParseTest<L20nCore.Objects.IfElseExpression>
				("x || y ? (ok) : 42");
		}

		[Test()]
		public void InvalidEntryParseTests()
		{
			var builder = new L20nCore.Internal.LocaleContext.Builder();
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
				() => Entry.Parse(NC("<foo($nope, $nope) { 'a parameter name needs to be unique' }>"), builder));
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
				() => Entry.Parse(NC("<invalid['index'] 'no index allowed when giving a stringValue'>"), builder));
			Assert.Throws<ParseException>(
				() => Entry.Parse(NC("<invalid['value'] 42>"), builder));
		}

		private T ExpressionParseTest<T>(string input) where T : L20nCore.Objects.L20nObject
		{
			var stream = new CharStream(input);
			T result = (T)Expression.Parse(stream).Eval();
			TypeAssert<T>(result);
			if (stream.InputLeft())
				throw new ParseException("stream is non-empty: " + stream.ReadUntilEnd());
			return result;
		}

		private void ExpressionParseTest(string expected, string input)
		{
			Assert.AreEqual(
				expected,
				ExpressionParseTest<L20nCore.Objects.StringOutput>(input).Value);
		}
		
		private void ExpressionParseTest(int expected, string input)
		{
			Assert.AreEqual(
				expected,
				ExpressionParseTest<L20nCore.Objects.Literal>(input).Value);
		}
		
		private void ExpressionParseTest(bool expected, string input)
		{
			Assert.AreEqual(
				expected,
				ExpressionParseTest<L20nCore.Objects.BooleanValue>(input).Value);
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
