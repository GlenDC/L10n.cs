using System;
using System.Collections.Generic;

namespace L20n
{
	namespace IO
	{
		public class Parser
		{
			private CharStream m_Stream;
			private Parsers.Entry m_EntryParser;

			public Parser(string file_name)
			{
				m_Stream = new CharStream(file_name);
				m_EntryParser = new Parsers.Entry(m_Stream);
			}
			
			public List<Types.Entity> Parse()
			{
				var entities = new List<Types.Entity>();

				Types.Entry entry;
				while(!m_Stream.EndOfStream())
				{
					entry = m_EntryParser.Parse();
					Console.WriteLine(entry.ToString());
					entities.AddRange(entry.Evaluate());
				}

				return entities;
			}
		}
	}
}

