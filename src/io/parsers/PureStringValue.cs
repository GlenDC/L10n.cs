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
			/// The combinator parser used to parse a simple string surounded by quotes.
			/// This is not the same as a StringValue and cannot contain expressions within expanders.
			/// </summary>
			public static class PureStringValue
			{
				public static string Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try
					{
						var output = "";
						var quote = Quote.Parse(stream);

						char c;
						
						while ((c = stream.PeekNext()) != '\0')
						{
							if (c == '\\')
							{
								output += stream.ForceReadNext();
							} else if (Quote.Peek(stream, quote))
							{
								break; // un-escaped quote means we're ending the string
							}

							output += stream.ForceReadNext();
						}
						
						Quote.Parse(stream, quote);
						
						return output;
					} catch (Exception e)
					{
						string msg = String.Format(
							"something went wrong parsing an <string_value> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new Exceptions.ParseException(msg, e);
					}
				}
				
				public static bool Peek(CharStream stream)
				{
					return Quote.Peek(stream);
				}
				
				public static bool PeekAndParse(CharStream stream, out string value)
				{
					if (StringValue.Peek(stream))
					{
						value = PureStringValue.Parse(stream);
						return true;
					}
					
					value = null;
					return false;
				}
			}
		}
	}
}
