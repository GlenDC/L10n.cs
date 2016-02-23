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

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{
			public class Entry
			{
				public static Types.Entry Parse(CharStream stream)
				{
					char c = stream.ForceReadNext("Entry-Parser: No Input found");
					switch (c) {
					case '/':
						return ParseComment(stream);

					default:
						throw new IOException("Entry-Parser: first char not valid: " + c);
					}
				}

				private static Types.Entry ParseComment(CharStream stream)
				{
					char c = stream.ForceReadNext("Entry-Parser: ParseComment has no Input Left!");
					if(c != '*')
						throw new IOException("Entry-Parser: ParseComment char is not valid: " + c);

					return Comment.Parse(stream);
				}
			}
		}
	}
}
