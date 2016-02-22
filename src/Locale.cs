using System;

namespace L20n
{
	public class Locale
	{
		public Locale()
		{
		}

		public void Import(String file_name)
		{
			CharStream stream;
			using(stream = new CharStream(file_name)) 
			{
				char c;
				while(stream.ReadNext(out c))
					Console.Write(c);
			}
		}
	}
}
