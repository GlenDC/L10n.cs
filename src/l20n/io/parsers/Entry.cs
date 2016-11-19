// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L20n
	{
		namespace IO
		{
			namespace Parsers
			{	
				/// <summary>
				/// The combinator parser used to parse an entry.
				/// An entry could be a comment or a section, but should usually be a message.
				/// </summary>
				public static class Entry
				{
					public static bool PeekAndParse(CharStream stream, out L20n.IO.AST.INode result)
					{
						if (Message.PeekAndParse(stream, out result))
							return true;
						
						if (Comment.PeekAndParse(stream, out result))
							return true;
						
						if (Section.PeekAndParse(stream, out result))
							return true;

						result = null;
						return false;
					}
				}
			}
		}
	}
}
