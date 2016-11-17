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
				namespace Expressions
				{
					/// <summary>
					/// An attribute expressions looks for an attribute,
					/// which means it points to some metadata within an Entity.
					/// </summary>
					public static class Attribute
					{
						public static AST.L10n.INode Parse(CharStream stream)
						{
							var startingPos = stream.Position;
						
							try
							{
								AST.L10n.INode root;
								if (!Property.PeekAndParse(stream, out root))
								{
									root = IdentifierExpression.Parse(stream);
								}
								stream.SkipString("::");

								// we either have an expression or a simple identifier
								AST.L10n.INode identifier;
								if (stream.SkipIfPossible('['))
								{
									identifier = Expression.Parse(stream);
									stream.SkipCharacter(']');
								} else
								{
									identifier = new AST.L10n.Identifier(Identifier.Parse(stream, false));
								}

								// We can also have optionally a property expression,
								// starting with a simple identifier or straight away with an expression
								AST.L10n.PropertyExpression propertyExpression = null;
								if (stream.SkipIfPossible('.'))
								{
									propertyExpression = Property.Parse(stream) as AST.L10n.PropertyExpression;
								} else if (stream.SkipIfPossible('['))
								{
									var expression = Expression.Parse(stream);
									propertyExpression = new AST.L10n.PropertyExpression(expression);
									stream.SkipCharacter(']');
									Property.Parse(stream, propertyExpression);
								}

								return new AST.L10n.AttributeExpression(root, identifier, propertyExpression);
							} catch (Exception e)
							{
								string msg = String.Format(
								"something went wrong parsing an <property_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
								throw new Exceptions.ParseException(msg, e);
							}
						}
					
						public static bool PeekAndParse(
						CharStream stream, out AST.L10n.INode expression)
						{
							if (stream.PeekReg(@"[$@]?\w+(\.\w+)*::"))
							{
								expression = Attribute.Parse(stream);
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
