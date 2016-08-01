// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{
			/// <summary>
			/// The combinator parser used to parse an HashItem,
			/// which makes up one element in a HashValue.
			/// </summary>
			public static class HashItem
			{
				public static AST.HashValue.Item Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try
					{
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
					} catch (Exception e)
					{
						string msg = String.Format(
							"something went wrong parsing a <hash_item> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new Exceptions.ParseException(msg, e);
					}
				}

				public static bool PeekAndParse(CharStream stream, out AST.HashValue.Item item)
				{
					if (stream.PeekNext() == '*' || Identifier.Peek(stream))
					{
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
