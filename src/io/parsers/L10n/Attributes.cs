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
			namespace L10n
			{
				/// <summary>
				/// The combinator parser used to parse an Attributes collection,
				/// which is used as extra information of an Entity (meta-data).
				/// </summary>
				public static class Attributes
				{
					public static AST.L10n.Attributes Parse(CharStream stream)
					{
						var startingPos = stream.Position;
					
						try
						{
							var attributes = new AST.L10n.Attributes();
							AST.L10n.Attributes.Item item;
							while (KeyValuePair.PeekAndParse(stream, out item))
							{
								attributes.AddItem(item);
							}
						
							return attributes;
						} catch (Exception e)
						{
							string msg = String.Format(
							"something went wrong parsing a <attributes> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
							throw new Exceptions.ParseException(msg, e);
						}
					}
				
					public static bool PeekAndParse(CharStream stream, out AST.L10n.Attributes value)
					{
						if (KeyValuePair.Peek(stream))
						{
							value = Attributes.Parse(stream);
							return true;
						}
					
						value = null;
						return false;
					}
				}
			}
		}
	}
}