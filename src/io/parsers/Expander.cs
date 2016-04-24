/**
 * This source file is part of the Commercial L20n Unity Plugin.
 *
 * Copyright (c) 2016 Glen De Cauwsemaecker (contact@glendc.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;

namespace L20nCore
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
				public static AST.INode Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					AST.INode expression = null;

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
					CharStream stream, out AST.INode expression)
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
