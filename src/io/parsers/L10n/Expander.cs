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
				/// The combinator parser used to parse an expression within a StringValue.
				/// </summary>
				public static class Expander
				{
					public static AST.L10n.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
						AST.L10n.INode expression = null;

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
							throw new Exceptions.ParseException(msg, e);
						}
					}

					public static bool PeekAndParse(
					CharStream stream, out AST.L10n.INode expression)
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
