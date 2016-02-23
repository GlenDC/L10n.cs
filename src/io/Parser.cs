/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 - 2017 Glen De Cauwsemaecker (contact@glendc.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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

