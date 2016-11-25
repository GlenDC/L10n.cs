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
		namespace FTL
		{
			namespace Parsers
			{	
				/// <summary>
				/// The combinator parser used to parse the body,
				/// the root AST Object of a L20n resource file.
				/// </summary>
				public static class Body
				{
					public static L20n.FTL.AST.Body Parse(CharStream stream)
					{
						L20n.FTL.AST.Body body = new L20n.FTL.AST.Body();
						L20n.FTL.AST.INode entry;

						while (Entry.PeekAndParse(stream, out entry))
						{
							body.AddEntry(entry);
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

						return body;
					}
				}
			}
		}
	}
}
