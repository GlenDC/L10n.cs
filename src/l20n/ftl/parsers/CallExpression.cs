// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;
using System.Collections.Generic;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace Parsers
			{	
				/// <summary>
				/// The combinator parser used to parse a call-expression.
				/// </summary>
				public static class CallExpression
				{
					public static L20n.FTL.AST.CallExpression Parse(CharStream stream, L20n.FTL.AST.StringPrimitive builtin)
					{
						// make sure its a valid builtin, otherwise we can't accept it :(
						builtin.Transform(L20n.FTL.AST.StringPrimitive.Kind.Builtin);

						stream.SkipCharacter('(');
						// arguments are optional, so could also be an empty list (never null though)
						List<L20n.FTL.AST.INode> arguments = ArgumentsList.Parse(stream);
						stream.SkipCharacter(')');

						return new L20n.FTL.AST.CallExpression(builtin, arguments);
					}
					
					public static bool Peek(CharStream stream)
					{
						return stream.PeekNext() == '(';
					}
				}
			}
		}
	}
}
