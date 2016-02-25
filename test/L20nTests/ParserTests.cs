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

			L20n.Types.Entry comment;
			// PeekAndParse can be used to make sure that
			// you only try to parse something that at least looks like a comment
			Assert.IsTrue(Comment.PeekAndParse(NC("/* A Comment */"), out comment));
			// When the start doesn't look like a comment it will simply return false
			Assert.IsFalse(Comment.PeekAndParse(NC("Hello"), out comment));
			// It will still throw an assert however,
			// if the start looks like a comment, but it turned out to be a trap
			Assert.Throws<IOException> (
				() => Comment.PeekAndParse(NC("/* What can go wrong"), out comment));

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

		private CharStream NC(string buffer)
		{
			return new CharStream(buffer);
		}
	}
}

