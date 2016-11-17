// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{	
			namespace L10n
			{
				/// <summary>
				/// The combinator parser used to parse a string identifier,
				/// which either becomes an Identifier or IdentifierExpression,
				/// depending on the context.
				/// </summary>
				public static class Identifier
				{
					public static string Parse(CharStream stream, bool allow_underscore)
					{
						string identifier;
						if (!Identifier.PeekAndParse(stream, out identifier, allow_underscore))
						{
							throw stream.CreateException(
							"expected to read an <identifier>, but non-word character was found");
						}

						return identifier;
					}

					public static bool Peek(CharStream stream)
					{
						return stream.PeekReg(@"[_a-zA-Z]");
					}

					public static bool PeekAndParse(CharStream stream, out string identifier, bool allow_underscore)
					{
						var reg = allow_underscore ? @"[_a-zA-Z]\w*" : @"[a-zA-Z]\w*";
						if (!stream.EndOfStream() && stream.ReadReg(reg, out identifier))
						{
							return true;
						}

						identifier = null;
						return false;
					}
				}
			}
		}
	}
}
