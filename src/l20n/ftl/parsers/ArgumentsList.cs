// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace Parsers
			{	
				/// <summary>
				/// The combinator parser used to parse a list of arguments (arglist).
				/// </summary>
				public static class ArgumentsList
				{
					public static List<L20n.FTL.AST.INode> Parse(CharStream stream)
					{
						List<L20n.FTL.AST.INode> arguments = new List<L20n.FTL.AST.INode>();

						while (stream.PeekNext() != ')')
						{
							WhiteSpace.Parse(stream);
							arguments.Add(Argument.Parse(stream));
							WhiteSpace.Parse(stream);
							if (stream.PeekNext() != ',')
								break; // exit early, as no more arguments are expected
							stream.Skip(); // skip ','
						}

						// make sure last non-ws char is ')',
						// otherwise something went wrong
						stream.SkipCharacter(')');

						return arguments;
					}
				}
			}
		}
	}
}
