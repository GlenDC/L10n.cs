using System;
using System.IO;

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{	
			public class WhiteSpace
			{
				public static void Parse(CharStream stream)
				{
					stream.SkipWhile(char.IsWhiteSpace);
				}
			}
		}
	}
}
