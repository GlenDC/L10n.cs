// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{
			namespace Expressions
			{
				/// <summary>
				/// A parser combinator to parse any kind of Identifier Expression,
				/// which can be a External or Global variable or simply a naked IDentifier.
				/// In case it is a Identifier, it might because a normal or expression one,
				/// but that depends on the context in which it is used.
				/// </summary>
				public static class IdentifierExpression
				{
					public static AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;

						try
						{
							AST.INode identifier;

							if (Variable.PeekAndParse(stream, out identifier))
								return identifier;
							
							if (Global.PeekAndParse(stream, out identifier))
								return identifier;

							return new AST.Identifier(
								Identifier.Parse(stream, true));
						} catch (Exception e)
						{
							string msg = String.Format(
								"something went wrong parsing an <identifier> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
							throw new Exceptions.ParseException(msg, e);
						}
					}

					public static bool Peek(CharStream stream)
					{
						return Identifier.Peek(stream)
							|| Variable.Peek(stream)
							|| Global.Peek(stream);
					}
				}
			}
		}
	}
}
