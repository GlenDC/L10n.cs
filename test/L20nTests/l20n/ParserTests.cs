// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using NUnit.Framework;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

using L20nCore.L20n.IO.Parsers;

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
				WhiteSpace.Parse(stream, false);
				// This will not read anything, but it's optional
				// so it will not give an exception
				WhiteSpace.Parse(stream, true);
				// This will fail as it's not optional
				Assert.Throws<ParseException>(
					() => WhiteSpace.Parse(stream, false));

				
				stream = NC("   a <- foo");
				
				// This will read until 'a'
				WhiteSpace.Parse(stream, false);
				// This will fail as it's not optional
				Assert.Throws<ParseException>(
					() => WhiteSpace.Parse(stream, false));
				Assert.AreEqual("a <- foo", stream.ReadUntilEnd());
			}

			[Test()]
			public void NewLineTests()
			{
				var stream = NC("\n\r\n\n\n");
				
				// This will read everything
				NewLine.Parse(stream, false);
				// This will not read anything, but it's optional
				// so it will not give an exception
				NewLine.Parse(stream, true);
				// This will fail as it's not optional
				Assert.Throws<ParseException>(
					() => NewLine.Parse(stream, false));
				
				
				stream = NC("\n\r\n\n\ra <- foo");
				
				// This will read until 'a'
				NewLine.Parse(stream, false);
				// This will fail as it's not optional
				Assert.Throws<ParseException>(
					() => NewLine.Parse(stream, false));
				Assert.AreEqual("a <- foo", stream.ReadUntilEnd());
			}
			
			[Test()]
			public void CommentTests()
			{
				var stream = NC("# a comment in expected form");
				
				// This will read everything
				Comment.Parse(stream);
				Assert.IsEmpty(stream.ReadUntilEnd());

				// this will fail
				Assert.Throws<ParseException>(
					() => Comment.Parse(NC("as it is not a comment")));
				// this will also fail
				Assert.Throws<ParseException>(
					() => Comment.Parse(NC("as # it is still not a comment")));

				// The Comment parser will read the entire stream
				// once it detirmined it's a legal comment
				stream = NC("# a comment in expected form\n... Some important stuff \r\n ... \n # end");
				Comment.Parse(stream);
				Assert.IsEmpty(stream.ReadUntilEnd());
			}
			
			private CharStream NC(string buffer)
			{
				return new CharStream(buffer);
			}
		}
	}
}
