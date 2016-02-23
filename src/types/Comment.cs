using System;
using System.Collections.Generic;

namespace L20n
{
	namespace Types
	{
		public class Comment : Entry
		{
			private string m_Content;

			public Comment(string content)
			{
				m_Content = content;
			}

			public override List<Entity> Evaluate()
			{
				return new List<Entity>();
			}
			
			public override string ToString()
			{
				return String.Format("/*{0}*/", m_Content);
			}
		}
	}
}

