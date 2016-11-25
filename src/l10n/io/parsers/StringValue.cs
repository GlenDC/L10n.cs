// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;
using System.Text.RegularExpressions;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace Parsers
			{	
				/// <summary>
				/// The parser combinator used to parse a StringValue,
				/// used as the value-part of an Entity entry.
				/// </summary>
				public static class StringValue
				{
					public static L10n.IO.AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
					
						try
						{
							// Parse the quote and store that in the AST of the stringValue
							var quote = Quote.Parse(stream);
							var value = new L10n.IO.AST.StringValue(quote);

							L10n.IO.AST.INode expression;
							char c;

							// Trim starting space
							WhiteSpace.Parse(stream, true);
			
							// as long as we have more characters left
							// we'll keep reading, and break from within this loop body
							// when we reached the same quote that started this entire parser logic.
							while ((c = stream.PeekNext()) != '\0')
							{
								if (c == '\\')
								{
									// skip the escape character && read the next one
									stream.Skip();
									c = stream.PeekNext();
									if (c == '\\' || c == '{' || c == '}' || c == '\'' || c == '"')
									{
										c = stream.ForceReadNext();
										value.appendChar(c);
									} else if (c == 'u')
									{
										// skip the starter character
										stream.Skip();
										// try to read unicode character
										value.appendChar(ReadUnicodeCharacter(stream));
									} else
									{
										var msg = String.Format(
										"character \\{0} is not a valid escape character", c);
										throw stream.CreateException(msg);
									}
									// unicode characters are also supported in the classical U+xxxxxx notation
								} else if (c == 'U' && stream.PeekNext(1) == '+')
								{
									// skip the starter characters
									stream.Skip(2);
									// try to read unicode character
									value.appendChar(ReadUnicodeCharacter(stream));
								} else if (c == '\n' || c == '\r') // newlines get converted to a space
								{
									stream.Skip();
									var lc = value.LastCharacter;
									// we add this dummy character, such that we wouldn't add a space in
									// languages that don't use a space in general, such as chinese.
									if (!Char.IsWhiteSpace(lc) && lc != L10n.IO.AST.StringValue.DummyNewlineWhitespaceCharacter)
										value.appendChar(L10n.IO.AST.StringValue.DummyNewlineWhitespaceCharacter);
								} else
								{
									if (Quote.Peek(stream, quote))
									{
										break; // un-escaped quote means we're ending the string
									} else if (Expander.PeekAndParse(stream, out expression))
									{
										value.appendExpression(expression);
									} else
									{
										c = stream.ForceReadNext();
										var lc = value.LastCharacter;
										if (!Char.IsWhiteSpace(c)
											|| (lc != L10n.IO.AST.StringValue.DummyNewlineWhitespaceCharacter
											&& !Char.IsWhiteSpace(lc)))
											value.appendChar(c);
									}
								}
							}

							// Eventually we expect exactly the same string back
							Quote.Parse(stream, quote);
						
							return value;
						} catch (Exception e)
						{
							string msg = String.Format(
							"something went wrong parsing an <string_value> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
							throw new ParseException(msg, e);
						}
					}

					public static bool Peek(CharStream stream)
					{
						return Quote.Peek(stream);
					}
				
					public static bool PeekAndParse(CharStream stream, out L10n.IO.AST.INode value)
					{
						if (StringValue.Peek(stream))
						{
							value = StringValue.Parse(stream);
							return true;
						}

						value = null;
						return false;
					}

					private static char ReadUnicodeCharacter(CharStream stream)
					{
						string unicodeValue;
						if (stream.ReadReg(s_RegUnicode, out unicodeValue))
						{
							return (char)Convert.ToInt32(unicodeValue, 16);
						} else
						{
							throw stream.CreateException("not a valid unicode character");
						}
					}

					private static readonly Regex s_RegUnicode = new Regex("(0|1)?[0-9a-fA-F]{4,5}");
				}
			}
		}
	}
}
