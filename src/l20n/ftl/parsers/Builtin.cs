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
				/// The combinator parser used to parse a builtin.
				/// </summary>
				public static class Builtin
				{
					public static L20n.FTL.AST.Builtin Parse(CharStream stream)
					{
						string value = stream.ForceReadReg(@"[A-Z_.?\-]+");
						return new L20n.FTL.AST.Builtin(value);
					}
				}
			}
		}
	}
}
