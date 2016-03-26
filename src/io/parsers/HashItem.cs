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
			public class HashItem
			{
				public static AST.HashValue.Item Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try {
						// check if a hash item is supposed to be a default
						bool isDefault = stream.SkipIfPossible('*');
						
						// parse the raw identifier (key)
						var identifier = Identifier.Parse(stream, false);
						
						// whitespace is optional
						WhiteSpace.Parse(stream, true);
						// the seperator char is required as it seperates the key and the value
						stream.SkipCharacter(':');
						// more optional whitespace
						WhiteSpace.Parse(stream, true);
						
						// get the actual value, which is identified by the key
						var value = Value.Parse(stream);
						
						return new AST.HashValue.Item(identifier, value, isDefault);
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing a <hash_item> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new Exceptions.ParseException(msg, e);
					}
				}

				public static bool PeekAndParse(CharStream stream, out AST.HashValue.Item item)
				{
					if (stream.PeekNext () == '*' || Identifier.Peek (stream)) {
						item = HashItem.Parse(stream);
						return true;
					}

					item = null;
					return false;
				}
			}
		}
	}
}
