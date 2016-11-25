// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

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
				/// The combinator parser used to parse a variable.
				/// </summary>
				public static class Variable
				{
					public static L20n.FTL.AST.Variable Parse(CharStream stream)
					{
						stream.SkipCharacter('$');
						L20n.FTL.AST.StringPrimitive keyword = Keyword.Parse(stream);
						return new L20n.FTL.AST.Variable(keyword);
					}

					public static bool PeekAndParse(CharStream stream, out L20n.FTL.AST.INode result)
					{
						if (stream.PeekNext() != '$')
						{
							result = null;
							return false;
						}

						result = Parse(stream);
						return true;
					}
				}
			}
		}
	}
}
