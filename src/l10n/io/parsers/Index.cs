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
				/// The combinator parser used to parse an Index,
				/// the optional part of an Entity, used to retrieve alternative
				/// default value based on one or multiple expressions.
				/// </summary>
				public static class Index
				{
					public static L10n.IO.AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
					
						try
						{
							// skip open char
							stream.SkipCharacter('[');

							// we need at least one index
							var index = new L10n.IO.AST.Index(ParseExpression(stream));

							// others are optional
							while (stream.SkipIfPossible(','))
							{
								index.AddIndex(ParseExpression(stream));
							}

							// skip close char
							stream.SkipCharacter(']');

							return index;
						} catch (Exception e)
						{
							string msg = String.Format(
							"something went wrong parsing an <index> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
							throw new ParseException(msg, e);
						}
					}

					private static L10n.IO.AST.INode ParseExpression(CharStream stream)
					{
						// optional white space
						WhiteSpace.Parse(stream, true);
						// first expression
						var index = Expression.Parse(stream);
						// optional white space
						WhiteSpace.Parse(stream, true);

						return index;
					}
				
					public static bool PeekAndParse(CharStream stream, out L10n.IO.AST.INode index)
					{
						if (stream.PeekNext() != '[')
						{
							index = null;
							return false;
						}
					
						index = Index.Parse(stream);
						return true;
					}
				}
			}
		}
	}
}
