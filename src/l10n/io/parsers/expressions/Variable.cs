// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

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
					/// The expression parser combinator used to parse a Variable,
					/// which is an L10nObject externally given by the user-environment.
					/// </summary>
					public static class Variable
					{
						public static L10n.IO.AST.INode Parse(CharStream stream)
						{
							stream.SkipCharacter('$');
							var identifier = Identifier.Parse(stream, false);
							return new L10n.IO.AST.Variable(identifier);
						}

						public static bool Peek(CharStream stream)
						{
							return stream.PeekNext() == '$';
						}

						public static bool PeekAndParse(
						CharStream stream, out L10n.IO.AST.INode variable)
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
