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
			Assert.Throws<IOException> (
				() => WhiteSpace.Parse(stream, false));
		}

		[Test()]
		public void CommentTests()
		{
			// Parsing a normal comment
			Comment.Parse(NC("/* A Comment */"));
			// Parsing a comment with a lot of stars
			Comment.Parse(NC("/********* Bling Bling **********/"));
			// This will fail as a comment was never found
			Assert.Throws<IOException> (
				() => Comment.Parse(NC("No Comment */")));
			// This will fail as the comment was never terminated
			Assert.Throws<IOException> (
				() => Comment.Parse(NC("Unfinished Comment */")));

			L20n.Types.AST.Entry entry;
			// PeekAndParse can be used to make sure that
			// you only try to parse something that at least looks like a comment
			Assert.IsTrue(Comment.PeekAndParse(NC("/* A Comment */"), out entry));
			// When the start doesn't look like a comment it will simply return false
			Assert.IsFalse(Comment.PeekAndParse(NC("Hello"), out entry));
			// It will still throw an assert however,
			// if the start looks like a comment, but it turned out to be a trap
			Assert.Throws<IOException> (
				() => Comment.PeekAndParse(NC("/* What can go wrong"), out entry));

			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<IOException>(() => Comment.Parse(NC("")));
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
			Assert.Throws<IOException> (
				() => Quote.Parse (s, quote));
			
			// Single Quotes can't be matched with Double Quotes
			s = NC("'\""); quote = Quote.Parse(s);
			Assert.Throws<IOException> (
				() => Quote.Parse (s, quote));
			
			// Matching pairs should give you a good night sleep
			s = NC("''"); quote = Quote.Parse(s);
			Assert.AreEqual(
				quote.ToString(),
				Quote.Parse(s, quote).ToString());
			
			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<IOException>(() => Quote.Parse(NC("")));
		}

		[Test()]
		public void RawIdentifierTests()
		{
			// an identifier is a string that only contains word-characters
			Assert.AreEqual("aBcDeFgH", RawIdentifier.Parse(NC("aBcDeFgH")));
			Assert.AreEqual("Hello_World", RawIdentifier.Parse(NC("Hello_World")));

			// white-spaces are not included in that
			Assert.AreEqual("Hello", RawIdentifier.Parse(NC("Hello World")));
			// neither or dashes
			Assert.AreEqual("glen", RawIdentifier.Parse(NC("glen-dc")));
			// starting with a non-word char will however make it fail
			Assert.Throws<IOException>(() => RawIdentifier.Parse(NC(" oh")));

			// You can also Parse identifiers in a fail-safe way
			string id;
			Assert.IsTrue(RawIdentifier.PeekAndParse(NC("Ho_Ho_Ho"), out id));
			Assert.AreEqual("Ho_Ho_Ho", id);
			Assert.IsFalse(RawIdentifier.PeekAndParse(NC(" fails"), out id));

			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<IOException>(() => RawIdentifier.Parse(NC("")));
		}
		
		[Test()]
		public void LiteralTests()
		{
			// any integer is a valid literal
			Assert.AreEqual("-123", Literal.Parse(NC("-123")).ToString());
			Assert.AreEqual("42", Literal.Parse(NC("+42")).ToString());
			Assert.AreEqual("7", Literal.Parse(NC("7")).ToString());

			// decimals will be ignored and make for
			// an invalid buffer later on
			Assert.AreEqual("5", Literal.Parse(NC("5.2")).ToString());
			Assert.Throws<IOException>(() => Literal.Parse(NC(".2")));
			
			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<IOException>(() => Literal.Parse(NC("")));
		}

		[Test()]
		public void HashValueTests()
		{
			L20n.Types.Internal.Expressions.HashValue hashValue;

			// hash tables can be pretty easy
			hashValue = Primary.Parse(NC("{hello:'world'}"))
				as L20n.Types.Internal.Expressions.HashValue;
			Assert.AreEqual("world", hashValue.Get("hello").ToString ());

			// hash tables cannot be empty though
			Assert.Throws<IOException>(() => HashValue.Parse(NC("{}")));

			// hash tables can also have a default
			hashValue = Primary.Parse(NC("{*what:'ok', yes: 'no'}"))
				as L20n.Types.Internal.Expressions.HashValue;
			Assert.AreEqual("ok", hashValue.Get("what").ToString());
			Assert.AreEqual("ok", hashValue.Get("something").ToString());
			Assert.AreEqual("no", hashValue.Get("yes").ToString());
			
			// hash tables cannot contain duplicate identifiers
			Assert.Throws<IOException>(() => Primary.Parse(NC("{a: 'a', a: 'b'}")));
			// multiple defaults are also not allowed
			Assert.Throws<IOException>(() => Primary.Parse(NC("{*a: 'a', *b: 'b'}")));
			
			// hash tables can also be nested
			hashValue = Primary.Parse(NC (@"{
				short: {
				  	*subjective: 'Loki',
				    objective: 'Loki',
				    possessive: 'Lokis',
				},
				long: 'Loki Mobile Client'
			}")) as L20n.Types.Internal.Expressions.HashValue;

			Assert.AreEqual("Loki Mobile Client", hashValue.Get("long").ToString());
			var shortValue = hashValue.Get("short") as L20n.Types.Internal.Expressions.HashValue;
			Assert.AreEqual("Lokis", shortValue.Get("possessive").ToString());
			Assert.AreEqual("Loki", shortValue.Get("unknown").ToString());

			// exception is thrown when no default is defined
			Assert.Throws<KeyNotFoundException> (() => hashValue.Get ("unknown"));
			
			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<IOException>(() => HashValue.Parse(NC("")));
		}

		[Test()]
		public void IdentifierExpressionTests()
		{
			// One identifier parser to rule them all (4)
			TypeAssert<L20n.Types.Internal.Expressions.Identifier>(
				Identifier.Parse(NC("identifier")));
			TypeAssert<L20n.Types.Internal.Expressions.Variable>(
				Identifier.Parse(NC("$normal_variable")));
			TypeAssert<L20n.Types.Internal.Expressions.Global>(
				Identifier.Parse(NC("@global_variable")));
			TypeAssert<L20n.Types.Internal.Expressions.This>(
				Identifier.Parse(NC("~")));

			// Anything that would fail the RawIdentifier tests
			// would also fail this one, as it wraps around
			Assert.Throws<IOException>(
				() => Identifier.Parse(NC(" no_prefix_space_allowed")));
			Assert.Throws<IOException>(
				() => Identifier.Parse(NC("-only_underscores_and_letters_are_allowed")));
			
			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<IOException>(() => Identifier.Parse(NC("")));
		}
		
		[Test()]
		public void PrimaryExpressionTests()
		{
			// One primary expression to rule them all

			// literals
			TypeAssert<L20n.Types.Internal.Expressions.Literal>(
				Primary.Parse(NC("-42")));
			TypeAssert<L20n.Types.Internal.Expressions.Literal>(
				Primary.Parse(NC("+10")));
			TypeAssert<L20n.Types.Internal.Expressions.Literal>(
				Primary.Parse(NC("123")));

			// string values
			TypeAssert<L20n.Types.Internal.Expressions.StringValue>(
				Primary.Parse(NC("\"Hello Dude!\"")));
			TypeAssert<L20n.Types.Internal.Expressions.StringValue>(
				Primary.Parse(NC("'this works as well'")));

			// hash values
			TypeAssert<L20n.Types.Internal.Expressions.HashValue>(
				Primary.Parse(NC("{hello:'world'}")));
			TypeAssert<L20n.Types.Internal.Expressions.HashValue>(
				Primary.Parse(NC("{dev: 'go develop' , debug: 'go debug',}")));
			TypeAssert<L20n.Types.Internal.Expressions.HashValue>(
				Primary.Parse(NC("{one:{a:'a',b:'b'},two:'2', three:'3'}")));

			// identifier expressions
			TypeAssert<L20n.Types.Internal.Expressions.Variable>(
				Primary.Parse(NC("$ola")));
			TypeAssert<L20n.Types.Internal.Expressions.Global>(
				Primary.Parse(NC("@hello")));
			TypeAssert<L20n.Types.Internal.Expressions.Identifier>(
				Primary.Parse(NC("bom_dia")));

			// all other type of input should fail
			Assert.Throws<IOException>(() => Primary.Parse(NC("     ")));
			Assert.Throws<IOException>(() => Primary.Parse(NC("<hello 'world'>")));
			
			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<IOException>(() => Primary.Parse(NC("")));
		}

		[Test()]
		public void KeyValuePairTests()
		{
			// key value pairs can be used with simple string values
			var pair = KeyValuePair.Parse(NC("hello: 'world'"));
			Assert.AreEqual("hello", pair.Identifier);
			Assert.AreEqual("world", pair.Value.ToString());
			Assert.IsNull(pair.Index);

			// hash values can also be used as values
			var person =
				KeyValuePair.Parse(NC("person: {age: '23', name: 'glen', *dummy: 'nope'}")).Value
					as L20n.Types.Internal.Expressions.HashValue;
			Assert.AreEqual("glen", person.Get("name").ToString());
			Assert.AreEqual("nope", person.Get("job").ToString());

			// TODO: test with indexes
			
			// passing in an EOF stream will give an <EOF> IOException
			Assert.Throws<IOException>(() => KeyValuePair.Parse(NC("")));
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

