using System;

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{
			public abstract class BaseParser
			{
				protected CharStream m_Stream;

				public BaseParser(CharStream stream)
				{
					m_Stream = stream;
				}

				public abstract Types.Entry Parse();
			}
		}
	}
}
