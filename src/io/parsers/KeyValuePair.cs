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
			/// The combinator parser used to parse an attribute,
			/// which can be used as extra information within an Entity.
			/// </summary>
			public static class KeyValuePair
			{
				public static AST.Attributes.Item Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try
					{	
						WhiteSpace.Parse(stream, false);

						// parse the raw identifier (key)
						var identifier = Identifier.Parse(stream, false);

						// parse the index if possible
						AST.INode index;
						Index.PeekAndParse(stream, out index);

						WhiteSpace.Parse(stream, true);
						// required seperator
						stream.SkipCharacter(':');

						WhiteSpace.Parse(stream, true);

						var valuePos = stream.Position;

						// the actual value (StringValue or HashTable)
						var value = Value.Parse(stream);

						if ((value as IO.AST.HashValue) == null && index != null)
						{
							string msg = String.Format(
								"an index was given, but a stringValue was given, while a hashValue was expected",
								stream.ComputeDetailedPosition(valuePos));
							throw new Exceptions.ParseException(msg);
						}
						
						return new AST.Attributes.Item(identifier, index, value);
					} catch (Exception e)
					{
						string msg = String.Format(
							"something went wrong parsing a <key_value_pair> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new Exceptions.ParseException(msg, e);
					}
				}

				public static bool Peek(CharStream stream)
				{
					return stream.PeekReg(@"\s+[a-zA-Z]");
				}
				
				public static bool PeekAndParse(CharStream stream, out AST.Attributes.Item item)
				{
					if (KeyValuePair.Peek(stream))
					{
						item = KeyValuePair.Parse(stream);
						return true;
					}
					
					item = null;
					return false;
				}
			}
		}
	}
}
