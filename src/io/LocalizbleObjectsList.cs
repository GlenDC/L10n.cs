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
using System.IO;
using System.Collections.Generic;

namespace L20n
{
	namespace IO
	{
		public class LocalizbleObjectsList
		{
			public static bool Parse(string path, List<L20n.Types.Entity> entities)
			{
				try {
					// We first pass it all into this temporary list
					// so that we only pass the entities on to the other entities,
					// once we know sure that we could parse the entire file
					// without any issues. (e.g. no parsing errors occured)
					using(CharStream stream = CharStream.CreateFromFile(path)) {
						Types.AST.Entry node;
						List<Types.Entity> newEntities = null;
						while(stream.InputLeft()) {
							// Skip WhiteSpace
							IO.Parsers.WhiteSpace.Parse(stream, true);
							
							// Read Entry
							node = Parsers.Entry.Parse(stream);
							if(node.Evaluate(out newEntities))
								entities.AddRange(newEntities);
						}

						return newEntities != null && newEntities.Count > 0;
					}

					return false;
				}
				catch(Exception exception) {
					throw new IOException(
						String.Format("couldn't parse <localizble_objects_list> from file: {0}", path),
						exception);
				}
			}
		}
	}
}
