using System;
using System.IO;

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{
			public class Entry
			{
				public static Types.Entry Parse(CharStream stream)
				{
					char c = stream.ForceReadNext("Entry-Parser: No Input found");
					switch (c) {
					case '/':
						return ParseComment(stream);

					default:
						throw new IOException("Entry-Parser: first char not valid: " + c);
					}
				}

				private static Types.Entry ParseComment(CharStream stream)
				{
					char c = stream.ForceReadNext("Entry-Parser: ParseComment has no Input Left!");
					if(c != '*')
						throw new IOException("Entry-Parser: ParseComment char is not valid: " + c);

					return Comment.Parse(stream);
				}
			}
		}
	}
}
