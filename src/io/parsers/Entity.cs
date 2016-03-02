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
			public class Entity
			{
				public static void Parse(
					CharStream stream, string identifier,
					L20n.Internal.ContextBuilder builder)
				{
					var startingPos = stream.Position;

					try {
						// an optional index is possible
						L20n.Objects.L20nObject index = null;
						Index.PeekAndParse(stream, out index);

						// White Space is required
						WhiteSpace.Parse(stream, false);

						// Now we need the actual value
						var value = Value.Parse(stream);

						// White Space is optional
						WhiteSpace.Parse(stream, true);

						stream.SkipCharacter('>');

						builder.AddEntity(identifier,
						    new L20n.Objects.Entity(index, value));
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing an <entity> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new L20n.Exceptions.ParseException(msg, e);
					}
				}
			}
		}
	}
}
