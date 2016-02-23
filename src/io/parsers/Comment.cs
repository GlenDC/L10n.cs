using System;
using System.IO;

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{	
			public class Comment
			{
				public static Types.Entry Parse(CharStream stream)
				{
					char c; string content = "";
					while (stream.ReadNext(out c)) {
						if(c == '*' && stream.SkipIfPossible('/')) {
							return new Types.Comment(content);
						}
						content += c;
					}

					throw new IOException("ERROR IN COMMENT");
				}
			}
		}
	}
}
