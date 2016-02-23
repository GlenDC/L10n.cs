using System;
using System.Collections.Generic;

namespace L20n
{
	public class Locale
	{
		List<Types.Entity> m_Entities;

		public Locale()
		{
			m_Entities = new List<Types.Entity>();
		}

		public void Import(String file_name)
		{
			var parser = new IO.Parser(file_name);
			m_Entities.AddRange(parser.Parse());
		}
	}
}
