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
				/// The combinator parser used to parse a number.
				/// </summary>
				public static class Number
				{
					public static L20n.FTL.AST.Number Parse(CharStream stream)
					{
						string rawValue = stream.ForceReadReg(@"[0-9]+(\.[0-9]+)?");
						return new L20n.FTL.AST.Number(rawValue);
					}

					public static bool PeekAndParse(CharStream stream, out L20n.FTL.AST.INode result)
					{
						if (!stream.PeekReg(@"[0-9]"))
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
