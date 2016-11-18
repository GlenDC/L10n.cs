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
				/// The combinator parser used to parse an expression within a StringValue.
				/// </summary>
				public static class Expander
				{
					public static L10n.IO.AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
						L10n.IO.AST.INode expression = null;

						try
						{
							// skip opening tags
							stream.SkipString("{{");
							WhiteSpace.Parse(stream, true);

							// parse actual expression
							expression = Expression.Parse(stream);

							// skip closing tags
							WhiteSpace.Parse(stream, true);
							stream.SkipString("}}");

							return expression;
						} catch (Exception e)
						{
							string msg = String.Format(
							"something went wrong parsing an <expander> starting at {0}, expression was: {1} ({2})",
							stream.ComputeDetailedPosition(startingPos),
							expression != null ? expression.Display() : "<null>",
							expression != null ? expression.GetType().ToString() : "null");
							throw new ParseException(msg, e);
						}
					}

					public static bool PeekAndParse(
					CharStream stream, out L10n.IO.AST.INode expression)
					{
						if (stream.PeekNextRange(2) == "{{")
						{
							expression = Expander.Parse(stream);
							return true;
						}

						expression = null;
						return false;
					}
				}
			}
		}
	}
}
