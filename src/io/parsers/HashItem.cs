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
			public class HashItem
			{
				public static HashItemValue Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try {
						// check if a hash item is supposed to be a default
						bool isDefault = stream.SkipIfPossible('*');
						
						// parse the raw identifier (key)
						var identifier = RawIdentifier.Parse(stream);
						
						// whitespace is optional
						WhiteSpace.Parse(stream, true);
						// the seperator char is required as it seperates the key and the value
						stream.SkipCharacter(':');
						// more optional whitespace
						WhiteSpace.Parse(stream, true);
						
						// get the actual value, which is identified by the key
						var value = Value.Parse(stream);
						
						return new HashItemValue(
							identifier.As<L20n.Objects.Identifier>().Value,
							value, isDefault);
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing a <hash_item> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new L20n.Exceptions.ParseException(msg, e);
					}
				}

				public static bool PeekAndParse(CharStream stream, out HashItemValue item)
				{
					if (stream.PeekNext () == '*' || RawIdentifier.Peek (stream)) {
						item = HashItem.Parse(stream);
						return true;
					}

					item = null;
					return false;
				}

				public class HashItemValue
				{
					public string Identifier { get; private set; }
					public L20n.Objects.L20nObject Value { get; private set; }
					public bool IsDefault { get; private set; }

					public HashItemValue(string id, L20n.Objects.L20nObject val, bool is_def)
					{
						Identifier = id;
						Value = val;
						IsDefault = is_def;
					}
				}
			}
		}
	}
}
