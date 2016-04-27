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
using System.Collections.Generic;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{
			/// <summary>
			/// The combinator parser used to parse an Attributes collection,
			/// which is used as extra information of an Entity (meta-data).
			/// </summary>
			public static class Attributes
			{
				public static AST.Attributes Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try
					{
						var attributes = new AST.Attributes();
						AST.Attributes.Item item;
						while (KeyValuePair.PeekAndParse(stream, out item))
						{
							attributes.AddItem(item);
						}
						
						return attributes;
					} catch (Exception e)
					{
						string msg = String.Format(
							"something went wrong parsing a <attributes> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new Exceptions.ParseException(msg, e);
					}
				}
				
				public static bool PeekAndParse(CharStream stream, out AST.Attributes value)
				{
					if (KeyValuePair.Peek(stream))
					{
						value = Attributes.Parse(stream);
						return true;
					}

					value = null;
					return false;
				}
			}
		}
	}
}
