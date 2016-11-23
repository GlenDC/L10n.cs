// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Exceptions;
using L20nCore.Common.IO;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace Parsers
			{
				/// <summary>
				/// The combinator parser used to parse a message (similar to L10n's entity).
				/// </summary>
				public static class Message
				{
					public static L20n.FTL.AST.Message Parse(CharStream stream)
					{
						throw new NotImplementedException();
					}
					
					public static bool PeekAndParse(CharStream stream, out L20n.FTL.AST.INode comment)
					{
						throw new NotImplementedException();
					}
				}
			}
		}
	}
}
