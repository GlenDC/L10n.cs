// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace Parsers
			{	
				/// <summary>
				/// The combinator parser used to parse a section.
				/// </summary>
				public static class Section
				{
					public static L20n.FTL.AST.Section Parse(CharStream stream)
					{
						stream.SkipString("[[");
						WhiteSpace.Parse(stream);

						var keyword = Keyword.Parse(stream);

						WhiteSpace.Parse(stream);
						stream.SkipString("]]");

						return new L20n.FTL.AST.Section(keyword);
					}

					public static void ParseAndForget(CharStream stream)
					{
						stream.SkipString("[[");
						// this does mean that for applications the section could be bullox, but that's fine
						// tooling should prevent such things
						stream.SkipWhile(IsNotNewLine);
					}
					
					public static bool PeekAndParse(CharStream stream, Context ctx, out L20n.FTL.AST.INode section)
					{
						if (!(stream.PeekNext() == '['))
						{
							section = null;
							return false;
						}

						if (ctx.ASTType == Context.ASTTypes.Full)
						{
							section = Parse(stream);
							return true;
						}

						ParseAndForget(stream);
						section = null;
						return true;
					}
					
					private static bool IsNotNewLine(char c)
					{
						return !NewLine.Predicate(c);
					}
				}
			}
		}
	}
}
