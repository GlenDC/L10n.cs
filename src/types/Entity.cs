using System;
using System.IO;
using System.Collections.Generic;

namespace L20n
{
	namespace Types
	{
		public class Entity : Entry
		{	
			public Entity()
			{
				// TODO
			}
			
			public override List<Entity> Evaluate()
			{
				var entities = new List<Entity>();
				entities.Add(this);
				return entities;
			}
			
			public override string ToString()
			{
				throw new IOException("TODO");
			}
		}
	}
}

