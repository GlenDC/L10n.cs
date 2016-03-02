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
using System.Collections.Generic;

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{	
			public class StringValue
			{
				public static L20n.Objects.L20nObject Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try {
						var quote = Quote.Parse(stream);
						var builder = new Builder();

						L20n.Objects.L20nObject expression;
						char c;
			
						while((c = stream.PeekNext()) != '\0') {
							if(c == '\\') {
								builder.appendChar(stream.ForceReadNext());
								builder.appendChar(stream.ForceReadNext());
							}
							else {
								if(Quote.Peek(stream, quote)) {
									break; // un-escaped quote means we're ending the string
								}
								else if(Expander.PeekAndParse(stream, out expression)) {
									builder.appendExpression(expression);
								}
								else {
									builder.appendChar(stream.ForceReadNext());
								}
							}
						}

						Quote.Parse(stream, quote);
						
						return builder.Build();
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing an <string_value> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new L20n.Exceptions.ParseException(msg, e);
					}
				}

				public static bool Peek(CharStream stream)
				{
					return Quote.Peek(stream);
				}
				
				public static bool PeekAndParse(CharStream stream, out L20n.Objects.L20nObject value)
				{
					if(StringValue.Peek(stream)) {
						value = StringValue.Parse(stream);
						return true;
					}

					value = null;
					return false;
				}

				private class Builder
				{
					private string m_Value;
					private	List<L20n.Objects.L20nObject> m_Expressions;
					
					public Builder()
					{
						m_Value = "";
						m_Expressions = new List<L20n.Objects.L20nObject>();
					}
					
					public void appendChar(char c)
					{
						m_Value += c;
					}
					
					public void appendString(string s)
					{
						m_Value += s;
					}
					
					public void appendExpression(L20n.Objects.L20nObject e)
					{
						appendString(String.Format ("{{0}}", m_Expressions.Count));
						m_Expressions.Add(e);
					}
					
					public L20n.Objects.StringValue Build()
					{
						return new L20n.Objects.StringValue(m_Value, m_Expressions);
					}
				}
			}
		}
	}
}
