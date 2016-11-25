// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;
using System.Text.RegularExpressions;

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
					public static readonly Regex Regex = new Regex(@"[A-Z_.?\-]+");

					public static L20n.FTL.AST.StringPrimitive Parse(CharStream stream)
					{
						string value = stream.ForceReadReg(Regex);
						return new L20n.FTL.AST.StringPrimitive(value, L20n.FTL.AST.StringPrimitive.Kind.Builtin);
					}
				}
			}
		}
	}
}
