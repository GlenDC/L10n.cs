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

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{	
			public class Comment
			{
				public static void Parse(CharStream stream)
				{
					if (!stream.SkipIfPossible ('/') || !(stream.SkipIfPossible ('*'))) {
						throw stream.CreateException(
							"a comment has to be opened with '/*'");
					}

					char c; string content = "";
					while (stream.ReadNext(out c)) {
						if(c == '*' && stream.SkipIfPossible('/')) {
							return;
						}
						content += c;
					}

					throw new L20n.Exceptions.ParseException(
						"a comment entry was opened, but not closed",
						stream.CreateEOFException());
				}

				public static bool PeekAndParse(CharStream stream)
				{
					if (stream.PeekNext() != '/' || stream.PeekNext(1) != '*') {
						return false;
					}

					Comment.Parse(stream);
					return true;
				}
			}
		}
	}
}
