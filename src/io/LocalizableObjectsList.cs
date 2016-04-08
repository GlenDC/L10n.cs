/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 Glen De Cauwsemaecker (contact@glendc.com)
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

namespace L20nCore
{
	namespace IO
	{
		/// <summary>
		/// The main L20n parser, that simply reads all entries of a given L20n Resource File,
		/// and adds them to the given <see cref="L20nCore.Internal.LocaleContext+Builder"/>. 
		/// </summary>
		public class LocalizableObjectsList
		{
			/// <summary>
			/// Reads the L20n resource at the given path, and adds all the entries found in that resource,
			/// to the given <c>builder</c>.
			/// Throws a <see cref="L20nCore.Exceptions.ParseException"/> in case something went wrong,
			/// during the parsing of the given resource.
			/// </summary>
			public static void ImportAndParse(string path, Internal.LocaleContext.Builder builder)
			{
				try
				{
					using (CharStream stream = CharStream.CreateFromFile(path))
					{
						while (stream.InputLeft())
						{
							// Skip WhiteSpace
							IO.Parsers.WhiteSpace.Parse(stream, true);
							
							// Read Entry
							Parsers.Entry.Parse(stream, builder);
						}
					}
				} catch (Exception exception)
				{
					throw new Exceptions.ParseException(
						String.Format("couldn't parse <localizble_objects_list> from file: {0}", path),
						exception);
				}
			}
		}
	}
}
