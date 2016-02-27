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
			public class KeyValuePair
			{
				public static Types.AST.KeyValuePair Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try {
						// the key
						var identifier = RawIdentifier.Parse(stream);

						// index is optional
						Types.AST.Index index;
						Index.PeekAndParse(stream, out index);

						// required seperator, surounded by optional whitespace
						WhiteSpace.Parse(stream, true);
						stream.SkipCharacter(':');
						WhiteSpace.Parse(stream, true);

						// the value
						var value = Value.Parse(stream);

						return new Types.AST.KeyValuePair (
							identifier, index, value);
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing an <key_value_pair> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new IOException(msg, e);
					}
				}
				
				public static bool PeekAndParse(
					CharStream stream, out Types.AST.KeyValuePair pair)
				{
					if (RawIdentifier.Peek(stream)) {
						pair = KeyValuePair.Parse(stream);
						return true;
					}
					
					pair = null;
					return false;
				}
			}
		}
	}
}
