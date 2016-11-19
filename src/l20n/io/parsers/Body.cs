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
		namespace IO
		{
			namespace Parsers
			{	
				/// <summary>
				/// The combinator parser used to parse the body,
				/// the root AST Object of a L20n resource file.
				/// </summary>
				public static class Body
				{
					public static List<L20n.IO.AST.INode> Parse(CharStream stream)
					{
						List<L20n.IO.AST.INode> nodes = new List<L20n.IO.AST.INode>();
						L20n.IO.AST.INode entry;

						while (Entry.PeekAndParse(stream, out entry))
						{
							nodes.Add(entry);
							if (NewLine.Parse(stream, true) == 0)
								break;
						}

						// should be empty now
						if (stream.InputLeft())
						{
							throw stream.CreateException(
								"expected EOF, while more content was found. " +
								"Any garbage at the bottom of your file?");
						}

						return nodes;
					}
				}
			}
		}
	}
}
