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
using System.IO;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{	
			/// <summary>
			/// The parser combinator used to parse a quote that starts and ends a StringValue.
			/// We support four different types of quote, even though they are doing the exact same thing for now.
			/// </summary>
			public static class Quote
			{
				public enum Type
				{
					Single,
					Double,
				}
				
				public class Info : IComparable
				{
					public Type Type { get; private set; }

					public Info(Type type)
					{
						Type = type;
					}

					public int CompareTo(object obj)
					{
						Info otherInfo = obj as Info;
						if (otherInfo != null)
						{
							if (this.Type == otherInfo.Type)
								return 0;
							else
								return this.Type == Type.Single ? -1 : 1;
						} else
						{
							throw new Exceptions.UnexpectedObjectException(
								"Object is not a Quote.Info");
						}
					}

					public override string ToString()
					{
						return this.Type == Type.Single ? "\'" : "\"";
					}
				}

				public static Info Parse(CharStream stream, Info expected = null)
				{
					string output;
					int pos = stream.Position;

					if (!stream.ReadReg("(\'|\")", out output))
					{
						throw new Exceptions.ParseException(
							String.Format(
							"expected to read a <quote> (starting at {0}), but no characters were left",
							stream.ComputeDetailedPosition(pos)));
					}

					Info info;
					if (output[0] == '"')
					{
						info = new Info(Type.Double);
					} else 
					{
						info = new Info(Type.Single);
					}

					if (expected != null && expected.CompareTo(info) != 0)
					{
						throw new Exceptions.ParseException(
							String.Format(
							"expected to read {0} (starting at {1}), but found {2}",
							expected.ToString(),
							stream.ComputeDetailedPosition(pos),
							info.ToString()));
					}

					return info;
				}

				public static bool Peek(CharStream stream, Quote.Info quote = null)
				{
					if (quote != null)
					{
						var raw = quote.ToString();
						var output = stream.PeekNextRange(raw.Length);

						return raw == output;
					}

					char next = stream.PeekNext();
					return next == '\'' || next == '\"';
				}
			}
		}
	}
}
