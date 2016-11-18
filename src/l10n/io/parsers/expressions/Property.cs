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
				namespace Expressions
				{
					/// <summary>
					/// A property expressions looks for a value within a hash-table,
					/// which means it points to a certain Entity (the root) and than defines
					/// all the different levels as identifiers and/or expressions, meaning that it can reference
					/// simply a value within the root or a value within the value or the root, and so on.
					/// </summary>
					public static class Property
					{
						public static L10n.IO.AST.INode Parse(CharStream stream, L10n.IO.AST.PropertyExpression property = null)
						{
							var startingPos = stream.Position;
						
							try
							{
								if (property == null)
								{
									var obj = IdentifierExpression.Parse(stream);
									property = new L10n.IO.AST.PropertyExpression(obj);
								}

								char c;
								// can be either a simple identifier ('.') or expression ('[')
								while (stream.SkipAnyIfPossible(new char[] {'.', '['}, out c))
								{
									if (c == '.')
									{
										property.Add(Identifier.Parse(stream, false));
									} else
									{
										WhiteSpace.Parse(stream, true);
										property.Add(Expression.Parse(stream));
										WhiteSpace.Parse(stream, true);
										stream.SkipCharacter(']');
									}
								}
							
								return property;
							} catch (Exception e)
							{
								string msg = String.Format(
								"something went wrong parsing an <property_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
								throw new ParseException(msg, e);
							}
						}
					
						public static bool PeekAndParse(
						CharStream stream, out L10n.IO.AST.INode expression)
						{
							if (stream.PeekReg(@"(\$|\@|\_|)?\w+(\.|\[)"))
							{
								expression = Property.Parse(stream);
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
}
