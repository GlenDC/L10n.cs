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
			public class Entity
			{
				public static Types.Entity Parse(CharStream stream, string identifier)
				{
					// TODO check for optional index first

					// White Space is required
					WhiteSpace.Parse(stream, false);

					// Now we need the actual value
					var value = Value.Parse(stream);

					// TODO more

					stream.SkipCharacter('>');

					return new Types.Entity(identifier, value);
				}
			}
		}
	}
}
