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
				/// The combinator parser used to parse a section.
				/// </summary>
				public static class Section
				{
					public static L20n.FTL.AST.Section Parse(CharStream stream)
					{
						stream.SkipString("[[");
						WhiteSpace.Parse(stream);

						var keyword = Keyword.Parse(stream);

						WhiteSpace.Parse(stream);
						stream.SkipString("]]");

						return new L20n.FTL.AST.Section(keyword);
					}
					
					public static bool PeekAndParse(CharStream stream, out L20n.FTL.AST.INode section)
					{
						if (!(stream.PeekNext() == '['))
						{
							section = null;
							return false;
						}

						section = Parse(stream);
						return true;
					}
				}
			}
		}
	}
}
