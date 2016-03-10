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

namespace L20n
{
	namespace IO
	{
		public class LocalizbleObjectsList
		{
			public static void Parse(string path, Internal.LocaleContext.Builder builder)
			{
				try {
					using(CharStream stream = CharStream.CreateFromFile(path)) {
						while(stream.InputLeft()) {
							// Skip WhiteSpace
							IO.Parsers.WhiteSpace.Parse(stream, true);
							
							// Read Entry
							Parsers.Entry.Parse(stream, builder);
						}
					}
				}
				catch(Exception exception) {
					throw new Exceptions.ParseException(
						String.Format("couldn't parse <localizble_objects_list> from file: {0}", path),
						exception);
				}
			}
		}
	}
}
