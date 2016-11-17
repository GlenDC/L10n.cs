// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

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
					/// The expression parser combinator used to parse a Variable,
					/// which is an L20nObject externally given by the user-environment.
					/// </summary>
					public static class Variable
					{
						public static AST.L10n.INode Parse(CharStream stream)
						{
							stream.SkipCharacter('$');
							var identifier = Identifier.Parse(stream, false);
							return new AST.L10n.Variable(identifier);
						}

						public static bool Peek(CharStream stream)
						{
							return stream.PeekNext() == '$';
						}

						public static bool PeekAndParse(
						CharStream stream, out AST.L10n.INode variable)
						{
							if (!Variable.Peek(stream))
							{
								variable = null;
								return false;
							}

							variable = Variable.Parse(stream);
							return true;
						}
					}
				}
			}
		}
	}
}
