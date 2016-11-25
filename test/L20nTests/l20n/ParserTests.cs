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
			
			[Test()]
			public void IdentifierTests()
			{
				// a normal (and best case example)
				Assert.IsNotNull(Identifier.Parse(NC("hello")));
				
				// other legal (but not always great) examples
				Assert.IsNotNull(Identifier.Parse(NC("this is valid")));
				Assert.IsNotNull(Identifier.Parse(NC("this_is_also_valid")));
				Assert.IsNotNull(Identifier.Parse(NC("this-is-also-valid")));
				Assert.IsNotNull(Identifier.Parse(NC("Could be a sentence.")));
				Assert.IsNotNull(Identifier.Parse(NC("Or a question?")));
				Assert.IsNotNull(Identifier.Parse(NC("Room 42")));
				Assert.IsNotNull(Identifier.Parse(NC("?")));
				Assert.IsNotNull(Identifier.Parse(NC(".-?_???")));
				
				// bad examples
				Assert.Throws<ParseException>(
					() => Identifier.Parse(NC("4 cannot start with a number")));
				Assert.Throws<ParseException>(
					() => Identifier.Parse(NC("@ is not allowed")));
				Assert.Throws<ParseException>(
					() => Identifier.Parse(NC("# is not allowed")));
				Assert.Throws<ParseException>(
					() => Identifier.Parse(NC("# is not allowed")));
				Assert.Throws<ParseException>(
					() => Identifier.Parse(NC(" cannot start with space")));
			}
			
			[Test()]
			public void BuiltinTests()
			{
				// a normal (and best case example)
				Assert.IsNotNull(Builtin.Parse(NC("NUMBER")));
				
				// other legal (but not always great) examples
				Assert.IsNotNull(Builtin.Parse(NC("SOME_BUILT-IN")));
				Assert.IsNotNull(Builtin.Parse(NC("?-._A-Z")));
				
				// bad examples
				Assert.Throws<ParseException>(
					() => Builtin.Parse(NC("4 cannot start with a number")));
				Assert.Throws<ParseException>(
					() => Builtin.Parse(NC("@ is not allowed")));
				Assert.Throws<ParseException>(
					() => Builtin.Parse(NC("# is not allowed")));
				Assert.Throws<ParseException>(
					() => Builtin.Parse(NC("# is not allowed")));
				Assert.Throws<ParseException>(
					() => Builtin.Parse(NC(" cannot start with space")));
				Assert.Throws<ParseException>( // cannot contain lowercase letters
					() => Builtin.Parse(NC("aNOPE")));
			}

			[Test()]
			public void VariableTests()
			{
				// As long as it's an identifier prefixed with '$' it's fine
				Assert.IsNotNull(Variable.Parse(NC("$hello")));
				Assert.IsNotNull(Variable.Parse(NC("$Whatever")));

				// otherwise we get an exception
				Assert.Throws<ParseException>( // no '$' prefix
					() => Variable.Parse(NC("nope")));
				Assert.Throws<ParseException>( // illegal identifier
					() => Variable.Parse(NC("$4")));
			}
			
			[Test()]
			public void MemberExpressionTests()
			{
				Assert.IsTrue(MemberExpression.Peek(NC("[")));
				Assert.IsFalse(MemberExpression.Peek(NC("foo")));

				var id = Identifier.Parse(NC("foo"));

				// MemberExpressionParsing starts from the '['
				Assert.AreEqual("foo[bar]", MemberExpression.Parse(NC("[bar]"), id).Display());
				Assert.AreEqual("foo[this_is-ok?42]", MemberExpression.Parse(NC("[this_is-ok?42]"), id).Display());
				
				// otherwise we get an exception
				Assert.Throws<ParseException>( // no '[' prefix
					() => MemberExpression.Parse(NC("nope"), id));
				Assert.Throws<ParseException>( // no ']' postfix
					() => MemberExpression.Parse(NC("[nope"), id));
				Assert.Throws<ParseException>( // illegal keyword
					() => MemberExpression.Parse(NC("[42]"), id));
			}
			
			[Test()]
			public void NumberTests()
			{
				// legal examples
				Assert.AreEqual("42", Number.Parse(NC("42")).Display());
				Assert.AreEqual("123.456", Number.Parse(NC("123.456")).Display());

				// bad examples that do not make it throw an exception,
				// but just stop parsing instead

				var nc = NC("42.");
				Assert.AreEqual("42", Number.Parse(nc).Display());
				Assert.AreEqual(".", nc.ReadUntilEnd());
				
				nc = NC("42,00");
				Assert.AreEqual("42", Number.Parse(nc).Display());
				Assert.AreEqual(",00", nc.ReadUntilEnd());
				
				// bad examples
				Assert.Throws<ParseException>(// '-' not allowed
					() => Number.Parse(NC("-42")));
				Assert.Throws<ParseException>(// '+' not allowed
					() => Number.Parse(NC("+42")));
				Assert.Throws<ParseException>(// only numbers allowed
				    () => Number.Parse(NC("hello")));
			}
			
			private CharStream NC(string buffer)
			{
				return new CharStream(buffer);
			}
		}
	}
}
