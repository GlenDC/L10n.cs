// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

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
				/// The parser combinator used to parse the value-part of an Entity entry.
				/// The value can be either a StringValue, or HashValue,
				/// which will be parsed by the parser combinator of on of those 2 specific values.
				/// </summary>
				public static class Value
				{
					public static L10n.IO.AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
					
						try
						{
							L10n.IO.AST.INode value;
							if (!Value.PeekAndParse(stream, out value))
							{
								throw new ParseException(
								"couldn't find valid <value> type");
							}

							return value;
						} catch (Exception e)
						{
							string msg = String.Format(
							"something went wrong parsing a <value> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
							throw new ParseException(msg, e);
						}
					}

					public static bool Peek(CharStream stream)
					{
						return StringValue.Peek(stream)
							|| HashValue.Peek(stream);
					}

					public static bool PeekAndParse(
					CharStream stream, out L10n.IO.AST.INode value)
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
