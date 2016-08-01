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
				/// A parser combinator to parse global expressions.
				/// These are objects that retrieve an object (part of a global list of objects)
				/// by reference and evaluates that.
				/// </summary>
				public static class Global
				{
					public static AST.INode Parse(CharStream stream)
					{
						stream.SkipCharacter('@');
						var identifier = Identifier.Parse(stream, false);
						return new AST.Global(identifier);
					}

					public static bool Peek(CharStream stream)
					{
						return stream.PeekNext() == '@';
					}
					
					public static bool PeekAndParse(
						CharStream stream, out AST.INode variable)
					{
						if (!Global.Peek(stream))
						{
							variable = null;
							return false;
						}
						
						variable = Global.Parse(stream);
						return true;
					}
				}
			}
		}
	}
}
