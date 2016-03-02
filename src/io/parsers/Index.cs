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
		namespace Parsers
		{	
			public class Index
			{
				public static L20n.Objects.L20nObject Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try {
						
						// skip open char
						stream.SkipCharacter('[');

						// optional white space
						WhiteSpace.Parse(stream, true);
						// first expression
						var index = Expression.Parse(stream);
						// optional white space
						WhiteSpace.Parse(stream, true);

						// skip close char
						stream.SkipCharacter(']');

						return index;
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing an <index> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new L20n.Exceptions.ParseException(msg, e);
					}
				}
				
				public static bool PeekAndParse(CharStream stream, out L20n.Objects.L20nObject index)
				{
					if (stream.PeekNext() != '[') {
						index = null;
						return false;
					}
					
					index = Index.Parse(stream);
					return true;
				}
			}
		}
	}
}
