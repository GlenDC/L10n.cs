using System;
using System.IO;

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{	
			public class Comment : BaseParser
			{
				public Comment(CharStream stream)
					: base(stream)
				{}
				
				public override Types.Entry Parse()
				{
					char c; string content = "";
					while (m_Stream.ReadNext(out c, true)) {
						if(c == '*' && m_Stream.SkipIfPossible('/')) {
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
