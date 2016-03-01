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
		namespace Parsers
		{
			public class Entry
			{
				public static Types.AST.Entry Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try {
						Types.AST.Entry entry;

						if(Comment.PeekAndParse(stream, out entry))
							return entry;

						// normally we keep the requirements of a format for a parser internal,
						// but in this case we have the same start for both a <macro> and an <entity>
						// so we simply have to make an exception in this case for performance reasons
						if (stream.SkipIfPossible('<')) {
							string identifier = RawIdentifier.Parse(stream);

							if(Macro.PeekAndParse(stream, identifier, out entry))
							   return entry;

							// now it NEEDS to be a entitiy, else our input is simply invalid
							// knowing that we are already on a path of no return
							// because of the fact that we started parsing '<' and an identifier.
							return Entity.Parse(stream, identifier);
						}

						// it has to be an import statement at this point
						return ImportStatement.Parse(stream);
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing an <entry> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new L20n.Exceptions.ParseException(msg, e);
					}
				}
			}
		}
	}
}
