using System;
using System.Collections.Generic;

namespace L20n
{
	namespace Types
	{
		public abstract class Entry
		{
			public Entry() {}
			public abstract List<Entity> Evaluate();
		}
	}
}

