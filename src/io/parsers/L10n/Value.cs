// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{	
			namespace L10n
			{
				/// <summary>
				/// The parser combinator used to parse the value-part of an Entity entry.
				/// The value can be either a StringValue, or HashValue,
				/// which will be parsed by the parser combinator of on of those 2 specific values.
				/// </summary>
				public static class Value
				{
					public static AST.L10n.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
					
						try
						{
							AST.L10n.INode value;
							if (!Value.PeekAndParse(stream, out value))
							{
								throw new Exceptions.ParseException(
								"couldn't find valid <value> type");
							}

							return value;
						} catch (Exception e)
						{
							string msg = String.Format(
							"something went wrong parsing a <value> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
							throw new Exceptions.ParseException(msg, e);
						}
					}

					public static bool Peek(CharStream stream)
					{
						return StringValue.Peek(stream)
							|| HashValue.Peek(stream);
					}

					public static bool PeekAndParse(
					CharStream stream, out AST.L10n.INode value)
					{
						if (StringValue.PeekAndParse(stream, out value))
							return true;
					
						if (HashValue.PeekAndParse(stream, out value))
							return true;

						return false;
					}
				}
			}
		}
	}
}
