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
				/// The combinator parser used to parse an identifier.
				/// </summary>
				public static class Identifier
				{
					public static L20n.FTL.AST.Identifier Parse(CharStream stream)
					{
						string value = stream.ForceReadReg(@"[a-zA-Z_.?\-][a-zA-Z0-9_.?\-]*");
						return new L20n.FTL.AST.Identifier(value);
					}

					public static bool PeekAndParse(CharStream stream, out L20n.FTL.AST.INode result)
					{
						if (!stream.PeekReg(@"[a-zA-Z.?\-]"))
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
