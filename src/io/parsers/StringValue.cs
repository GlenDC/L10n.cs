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
using System.Collections.Generic;

namespace L20nCore
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
				public static AST.INode Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try
					{
						// Parse the quote and store that in the AST of the stringValue
						var quote = Quote.Parse(stream);
						var value = new AST.StringValue(quote);

						AST.INode expression;
						char c;
			
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
								if (!Char.IsWhiteSpace(lc) && lc != AST.StringValue.DummyNewlineWhitespaceCharacter)
									value.appendChar(AST.StringValue.DummyNewlineWhitespaceCharacter);
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
									if(!Char.IsWhiteSpace(c)
									   	|| (lc != AST.StringValue.DummyNewlineWhitespaceCharacter
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
						throw new Exceptions.ParseException(msg, e);
					}
				}

				public static bool Peek(CharStream stream)
				{
					return Quote.Peek(stream);
				}
				
				public static bool PeekAndParse(CharStream stream, out AST.INode value)
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
					if (stream.ReadReg("(0|1)?[0-9a-fA-F]{4,5}", out unicodeValue))
					{
						return (char)Convert.ToInt32(unicodeValue, 16);
					} else
					{
						throw stream.CreateException("not a valid unicode character");
					}
				}
			}
		}
	}
}
