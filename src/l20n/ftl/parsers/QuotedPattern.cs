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
				/// The combinator parser used to parse a quoted-pattern.
				/// A quoted-pattern can be either quoted-text or a placeable.
				/// </summary>
				public static class QuotedPattern
				{
					public static L20n.FTL.AST.INode Parse(CharStream stream)
					{
						throw new NotImplementedException();
					}
					
					public static bool PeekAndParse(CharStream stream, out L20n.FTL.AST.INode result)
					{
						if (stream.PeekNext() != '"')
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
