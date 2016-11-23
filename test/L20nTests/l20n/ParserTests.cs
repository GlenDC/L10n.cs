// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using NUnit.Framework;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

using L20nCore.L20n.FTL.Parsers;

namespace L20nCoreTests
{
	namespace L20n
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
				var stream = NC("   \t\t  ");
				
				// This will read everything
				WhiteSpace.Parse(stream);
				// This will not read anything, but it's optional
				// so it will not give an exception
				Assert.AreEqual(0, WhiteSpace.Parse(stream));

				
				stream = NC("   a <- foo");
				
				// This will read until 'a'
				WhiteSpace.Parse(stream);
				Assert.AreEqual(0, WhiteSpace.Parse(stream));
				Assert.AreEqual("a <- foo", stream.ReadUntilEnd());
			}

			[Test()]
			public void NewLineTests()
			{
				var stream = NC("\n\r\n\n\n");
				
				// This will read everything
				NewLine.Parse(stream);
				// This will fail as it's not optional
				Assert.Throws<ParseException>(
					() => NewLine.Parse(stream));
				
				
				stream = NC("\n\r\n\n\ra <- foo");
				
				// This will read until 'a'
				NewLine.Parse(stream);
				// This will fail as it's not optional
				Assert.Throws<ParseException>(
					() => NewLine.Parse(stream));
				Assert.AreEqual("a <- foo", stream.ReadUntilEnd());
			}
			
			[Test()]
			public void CommentTests()
			{
				var stream = NC("# a comment in expected form");
				
				// This will read everything
				Assert.IsNotNull(Comment.Parse(stream));
				Assert.IsEmpty(stream.ReadUntilEnd());

				// this will fail
				Assert.Throws<ParseException>(
					() => Comment.Parse(NC("as it is not a comment")));
				// this will also fail
				Assert.Throws<ParseException>(
					() => Comment.Parse(NC("as # it is still not a comment")));

				// The Comment parser will read the entire stream
				// once it detirmined it's a legal comment
				stream = NC("# a comment in expected form\n# new comment");
				Assert.IsNotNull(Comment.Parse(stream));
				Assert.AreEqual("\n# new comment", stream.ReadUntilEnd());
			}
			
			[Test()]
			public void SectionTests()
			{
				L20nCore.L20n.FTL.AST.INode node;

				// a section starts with '[['
				Assert.IsFalse(
					Section.PeekAndParse(NC("not a section"), out node));
				Assert.Throws<ParseException>(
					() => Section.PeekAndParse(NC("[not a section either]"), out node));
				Assert.IsTrue(Section.PeekAndParse(NC("[[ a section ]]"), out node));
				Assert.AreEqual("[[ a section ]]", node.Display());
				Assert.Throws<ParseException>(
					() => Section.PeekAndParse(NC("[[ needs to end with double brackets"), out node));
				Assert.Throws<ParseException>(
					() => Section.PeekAndParse(NC("[[ needs to end with double brackets ]"), out node));

				// check keyword tests to see examples
				// and to see what is and what is not allowed in between the double square brackets
				Assert.IsNotNull(Section.Parse(NC("[[section]]")));
			}

			[Test()]
			public void KeywordTests()
			{
				// a normal (and best case example)
				Assert.IsNotNull(Keyword.Parse(NC("hello")));

				// other legal (but not always great) examples
				Assert.IsNotNull(Keyword.Parse(NC("this is valid")));
				Assert.IsNotNull(Keyword.Parse(NC("this_is_also_valid")));
				Assert.IsNotNull(Keyword.Parse(NC("this-is-also-valid")));
				Assert.IsNotNull(Keyword.Parse(NC("Could be a sentence.")));
				Assert.IsNotNull(Keyword.Parse(NC("Or a question?")));
				Assert.IsNotNull(Keyword.Parse(NC("Room 42")));

				// bad examples
				Assert.Throws<ParseException>(
					() => Keyword.Parse(NC("4 cannot start with a number")));
				Assert.Throws<ParseException>(
					() => Keyword.Parse(NC("@ is not allowed")));
				Assert.Throws<ParseException>(
					() => Keyword.Parse(NC("# is not allowed")));
				Assert.Throws<ParseException>(
					() => Keyword.Parse(NC("# is not allowed")));
				Assert.Throws<ParseException>(
					() => Keyword.Parse(NC(" cannot start with space")));
			}
			
			private CharStream NC(string buffer)
			{
				return new CharStream(buffer);
			}
		}
	}
}
