// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;
using System.Text.RegularExpressions;

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
					/// An attribute expressions looks for an attribute,
					/// which means it points to some metadata within an Entity.
					/// </summary>
					public static class Attribute
					{
						public static L10n.IO.AST.INode Parse(CharStream stream)
						{
							var startingPos = stream.Position;
						
							try
							{
								L10n.IO.AST.INode root;
								if (!Property.PeekAndParse(stream, out root))
								{
									root = IdentifierExpression.Parse(stream);
								}
								stream.SkipString("::");

								// we either have an expression or a simple identifier
								L10n.IO.AST.INode identifier;
								if (stream.SkipIfPossible('['))
								{
									identifier = Expression.Parse(stream);
									stream.SkipCharacter(']');
								} else
								{
									identifier = new L10n.IO.AST.Identifier(Identifier.Parse(stream, false));
								}

								// We can also have optionally a property expression,
								// starting with a simple identifier or straight away with an expression
								L10n.IO.AST.PropertyExpression propertyExpression = null;
								if (stream.SkipIfPossible('.'))
								{
									propertyExpression = Property.Parse(stream) as L10n.IO.AST.PropertyExpression;
								} else if (stream.SkipIfPossible('['))
								{
									var expression = Expression.Parse(stream);
									propertyExpression = new L10n.IO.AST.PropertyExpression(expression);
									stream.SkipCharacter(']');
									Property.Parse(stream, propertyExpression);
								}

								return new L10n.IO.AST.AttributeExpression(root, identifier, propertyExpression);
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
							if (stream.PeekReg(s_RegPeek))
							{
								expression = Attribute.Parse(stream);
								return true;
							}
						
							expression = null;
							return false;
						}

						
						private static readonly Regex s_RegPeek = new Regex(@"[$@]?\w+(\.\w+)*::");
					}
				}
			}
		}
	}
}
