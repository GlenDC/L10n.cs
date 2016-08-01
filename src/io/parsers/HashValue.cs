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
			/// The combinator parser used to parse an HashValue,
			/// which is used as the value of an Entity.
			/// </summary>
			public static class HashValue
			{
				public static AST.INode Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try
					{
						// skip opening tag
						stream.SkipCharacter('{');

						// at least 1 hashItem is required with optional whitespace surrounding it
						var hashValue = new AST.HashValue();
						WhiteSpace.Parse(stream, true);
						hashValue.AddItem(HashItem.Parse(stream));
						WhiteSpace.Parse(stream, true);

						// parse all other (optional) hashItems
						while (stream.SkipIfPossible(','))
						{
							if (!HashValue.ParseHashItem(stream, hashValue))
							{
								// if we have a  trailing comma, it will be break here
								break;
							}
						}

						// skip closing tag
						stream.SkipCharacter('}');

						return hashValue;
					} catch (Exception e)
					{
						string msg = String.Format(
							"something went wrong parsing a <hash_value> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new Exceptions.ParseException(msg, e);
					}
				}

				public static bool Peek(CharStream stream)
				{
					return stream.PeekNext() == '{';
				}

				public static bool PeekAndParse(
					CharStream stream, out AST.INode value)
				{
					if (HashValue.Peek(stream))
					{
						value = HashValue.Parse(stream);
						return true;
					}

					value = null;
					return false;
				}
				
				private static bool ParseHashItem(CharStream stream, AST.HashValue hash_value)
				{
					// optional whitespace
					WhiteSpace.Parse(stream, true);
					// parse actual hashItem
					AST.HashValue.Item item;

					// we might be dealing with a trailing comma
					if (!HashItem.PeekAndParse(stream, out item))
					{
						return false;
					}

					// add acutal item
					hash_value.AddItem(item);

					// optional whitespace
					WhiteSpace.Parse(stream, true);

					return true;
				}
			}
		}
	}
}
