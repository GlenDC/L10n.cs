// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Exceptions;
using L20nCore.Common.IO;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace Parsers
			{
				/// <summary>
				/// The combinator parser used to parse a comment.
				/// </summary>
				public static class Comment
				{
					public static L20n.FTL.AST.Comment Parse(CharStream stream)
					{
						stream.SkipCharacter('#');
						string value = stream.ReadWhile(IsNotNewLine);
						return new L20n.FTL.AST.Comment(value);
					}

					public static void ParseAndForget(CharStream stream)
					{
						stream.SkipCharacter('#');
						stream.SkipWhile(IsNotNewLine);
					}

					public static bool Peek(CharStream stream)
					{
						return stream.PeekNext() == '#';
					}
						
					public static bool PeekAndParse(CharStream stream, Context ctx, out L20n.FTL.AST.INode comment)
					{
						if (stream.PeekNext() != '#')
						{
							comment = null;
							return false;
						}

						if (ctx.ASTType == Context.ASTTypes.Full)
						{
							comment = Parse(stream);
							return true;
						}

						ParseAndForget(stream);
						comment = null;
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
