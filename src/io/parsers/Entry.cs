using System;
using System.IO;

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{
			// entry	:	entity | macro | comment | statement ;
			
			// entity	:	'<' identifier index? WS+ value attributes? WS? '>' ;
			// macro	:	'<' identifier '(' WS? ( variable WS? ( ',' WS? variable WS? )* )? ')' WS+ '{' WS? expression WS? '}' WS? '>' ;
			// comment	:	'/*' .*? '*/' ;
			// statement :	import_statement ;
			// import_statement	:	'import(' WS? expression WS? ')' ;

			public class Entry : BaseParser
			{
				public Entry(CharStream stream)
					: base(stream)
				{}

				public override Types.Entry Parse()
				{
					char c;
					c = m_Stream.ForceReadNext("Entry-Parser: first char not found");

					switch (c) {
					case '/':
						c = m_Stream.ForceReadNext("Entry-Parser: second char not found");
						if(c != '*')
							throw new IOException("Entry-Parser: second char not valid: ");
						return new Comment(m_Stream).Parse();
					default:
						throw new IOException("Entry-Parser: first char not valid: ");
					}
				}
			}
		}
	}
}
