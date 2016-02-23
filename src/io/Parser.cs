using System;
using System.Collections.Generic;

namespace L20n
{
	namespace IO
	{
		public class Parser
		{
			private CharStream m_Stream;

			public Parser(string file_name)
			{
				m_Stream = new CharStream(file_name);
			}
			
			public List<Types.Entity> Parse()
			{
				var entities = new List<Types.Entity>();

				Types.Entry entry;
				while(m_Stream.InputLeft())
				{
					// Skip WhiteSpace
					Parsers.WhiteSpace.Parse(m_Stream);

					// Read Entry
					entry = Parsers.Entry.Parse(m_Stream);
					Console.WriteLine(entry.ToString());
					entities.AddRange(entry.Evaluate());
				}

				return entities;
			}
		}
	}
}

