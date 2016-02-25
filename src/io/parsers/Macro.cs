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
			public class Macro
			{
				public static Types.Entry Parse(CharStream stream, string identifier)
				{
					throw stream.CreateException("started to read macro, but there is no supported for them");
				}

				public static bool PeekAndParse(CharStream stream, string identifier, out Types.Entry macro)
				{
					if (stream.PeekNext () != '(') {
						macro = null;
						return false;
					}

					macro = Macro.Parse(stream, identifier);
					return true;
				}
			}
		}
	}
}
