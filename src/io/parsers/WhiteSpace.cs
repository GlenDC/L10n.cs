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

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{	
			/// <summary>
			/// The parser combinator used to parse all the whitespace.
			/// The resulting output does not get stored.
			/// </summary>
			public static class WhiteSpace
			{
				public static int Parse(CharStream stream, bool optional)
				{
					int pos = stream.Position;
					int n = stream.SkipWhile(char.IsWhiteSpace);
					if (!optional && n == 0)
					{
						throw stream.CreateException(
							"at least one whitespace character is required",
							pos - stream.Position);
					}

					return n;
				}
			}
		}
	}
}
