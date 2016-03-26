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
			public class StringValue
			{
				public static AST.INode Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try {
						var quote = Quote.Parse(stream);
						var value = new AST.StringValue(quote);

						AST.INode expression;
						char c;
			
						while((c = stream.PeekNext()) != '\0') {
							if(c == '\\') {
								value.appendChar(stream.ForceReadNext());
								value.appendChar(stream.ForceReadNext());
							}
							else {
								if(Quote.Peek(stream, quote)) {
									break; // un-escaped quote means we're ending the string
								}
								else if(Expander.PeekAndParse(stream, out expression)) {
									value.appendExpression(expression);
								}
								else {
									value.appendChar(stream.ForceReadNext());
								}
							}
						}

						Quote.Parse(stream, quote);
						
						return value;
					}
					catch(Exception e) {
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
					if(StringValue.Peek(stream)) {
						value = StringValue.Parse(stream);
						return true;
					}

					value = null;
					return false;
				}
			}
		}
	}
}
