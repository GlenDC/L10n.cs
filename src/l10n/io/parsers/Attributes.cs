// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace Parsers
			{
				/// <summary>
				/// The combinator parser used to parse an Attributes collection,
				/// which is used as extra information of an Entity (meta-data).
				/// </summary>
				public static class Attributes
				{
					public static L10n.IO.AST.Attributes Parse(CharStream stream)
					{
						var startingPos = stream.Position;
					
						try
						{
							var attributes = new L10n.IO.AST.Attributes();
							L10n.IO.AST.Attributes.Item item;
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
							throw new ParseException(msg, e);
						}
					}
				
					public static bool PeekAndParse(CharStream stream, out L10n.IO.AST.Attributes value)
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