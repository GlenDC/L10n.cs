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
using System.IO;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{	
			public class Quote
			{
				public enum Type
				{
					Single,
					Double,
				}
				public enum LineType
				{
					Single,
					Multi,
				}
				
				public class Info : IComparable
				{
					public Type Type { get; private set; }
					public LineType LineType {get; private set; }

					public Info(Type type, LineType lineType)
					{
						Type = type;
						LineType = lineType;
					}

					public int CompareTo(object obj) {
						if (obj == null) return 1;
						
						Info otherInfo = obj as Info;
						if (otherInfo != null) {
							if(this.Type == otherInfo.Type) {
								if(this.LineType == otherInfo.LineType) {
									return 0;
								}
								else {
									return (this.LineType == LineType.Single ? -1 : 1);
								}
							}
							else {
								return (this.Type == Type.Single ? -1 : 1);
							}
						} else {
							throw new Exceptions.UnexpectedObjectException(
								"Object is not a Quote.Info");
						}
					}

					public override string ToString()
					{
						char c = (this.Type == Type.Single ? '\'' : '"');
						return new String(c, (this.LineType == LineType.Single ? 1 : 3));
					}
				}

				public static Info Parse(CharStream stream, Info expected = null)
				{
					string output;
					int pos = stream.Position;

					if (!stream.ReadReg("(\"\"\"|\'\'\'|\'|\")", out output)) {
						throw new Exceptions.ParseException(
							String.Format(
							"expected to read a <quote> (starting at {0}), but found invalid characters",
							stream.ComputeDetailedPosition(pos)));
					}

					
					var lineType = output.Length == 1 ? LineType.Single : LineType.Multi;
					Info info;
					if (output [0] == '"') {
						info = new Info (Type.Double, lineType);
					} else {
						info = new Info (Type.Single, lineType);
					}

					if (expected != null && expected.CompareTo(info) != 0) {
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
					if (quote != null) {
						var raw = quote.ToString();
						var output = stream.PeekNextRange(raw.Length);

						return raw == output;
					}

					char next = stream.PeekNext ();
					return next == '\'' || next == '"';
				}
			}
		}
	}
}
