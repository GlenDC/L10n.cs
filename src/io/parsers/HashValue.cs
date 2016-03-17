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

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{
			public class HashValue
			{
				public static AST.INode Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try {
						// skip opening tag
						stream.SkipCharacter('{');

						// at least 1 hashItem is required with optional whitespace surrounding it
						var hashValue = new AST.HashValue();
						WhiteSpace.Parse(stream, true);
						hashValue.AddItem(HashItem.Parse(stream));
						WhiteSpace.Parse(stream, true);

						// parse all other (optional) hashItems
						while (stream.SkipIfPossible(',')) {
							if(!HashValue.ParseHashItem(stream, hashValue)) {
								// if we have a  trailing comma, it will be break here
								break;
							}
						}

						// skip closing tag
						stream.SkipCharacter('}');

						return hashValue;
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing a <hash_value> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new Exceptions.ParseException(msg, e);
					}
				}

				public static bool Peek(CharStream stream)
				{
					return stream.PeekNext () == '{';
				}

				public static bool PeekAndParse(
					CharStream stream, out AST.INode value)
				{
					if (HashValue.Peek(stream)) {
						value = HashValue.Parse(stream);
						return true;
					}

					value = null;
					return false;
				}
				
				private static bool ParseHashItem(CharStream stream, AST.HashValue hash_value)
				{
					// optional whitespace
					WhiteSpace.Parse (stream, true);
					// parse actual hashItem
					AST.HashValue.Item item;

					// we might be dealing with a trailing comma
					if (!HashItem.PeekAndParse (stream, out item)) {
						return false;
					}

					// add acutal item
					hash_value.AddItem (item);

					// optional whitespace
					WhiteSpace.Parse (stream, true);

					return true;
				}
			}
		}
	}
}
