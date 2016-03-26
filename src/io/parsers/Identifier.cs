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
			public class Identifier
			{
				public static string Parse(CharStream stream, bool allow_underscore)
				{
					string identifier;
					if(!Identifier.PeekAndParse(stream, out identifier, allow_underscore)) {
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
					if (!stream.EndOfStream() && stream.ReadReg(reg, out identifier)) {
						return true;
					}

					identifier = null;
					return false;
				}
			}
		}
	}
}
