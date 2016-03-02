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

			public class HashValue
			{
				public static L20n.Objects.L20nObject Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try {
						// skip opening tag
						stream.SkipCharacter('{');

						// at least 1 hashItem is required with optional whitespace surrounding it
						var builder = new Builder();
						WhiteSpace.Parse(stream, true);
						builder.Add(HashItem.Parse(stream));
						WhiteSpace.Parse(stream, true);

						// parse all other (optional) hashItems
						while (stream.SkipIfPossible(',')) {
							if(!HashValue.ParseHashItem(stream, builder)) {
								// if we have a  trailing comma, it will be break here
								break;
							}
						}

						// skip closing tag
						stream.SkipCharacter('}');

						return builder.Build();
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing a <hash_value> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new L20n.Exceptions.ParseException(msg, e);
					}
				}

				public static bool Peek(CharStream stream)
				{
					return stream.PeekNext () == '{';
				}

				public static bool PeekAndParse(
					CharStream stream, out L20n.Objects.L20nObject value)
				{
					if (HashValue.Peek(stream)) {
						value = HashValue.Parse(stream);
						return true;
					}

					value = null;
					return false;
				}
				
				private static bool ParseHashItem(CharStream stream, Builder builder)
				{
					// optional whitespace
					WhiteSpace.Parse(stream, true);
					// parse actual hashItem
					HashItem.HashItemValue item;

					// we might be dealing with a trailing comma
					if (!HashItem.PeekAndParse(stream, out item)) {
						return false;
					}

					// add acutal item
					builder.Add(item);

					// optional whitespace
					WhiteSpace.Parse(stream, true);

					return true;
				}

				private class Builder
				{
					private Dictionary<string, L20n.Objects.L20nObject> m_Values;
					private string m_Default;

					public Builder()
					{
						m_Values = new Dictionary<string, L20n.Objects.L20nObject>();
						m_Default = null;
					}

					public void Add(HashItem.HashItemValue value)
					{
						try {
							m_Values.Add(value.Identifier, value.Value);
						}
						catch(ArgumentException) {
							throw new L20n.Exceptions.ImportException(
								String.Format(
									"<hash_item> with identifier {0} can't be added, as identifier isn't unique",
									value.Identifier));
						}

						if(value.IsDefault) {
							if(m_Default != null) {
								throw new L20n.Exceptions.ImportException(
									String.Format("<hash_item> already has a default with key {0}", m_Default));
							}

							m_Default = value.Identifier;
						}
					}

					public L20n.Objects.HashValue Build()
					{
						return new L20n.Objects.HashValue(m_Values, m_Default);
					}
				}
			}
		}
	}
}
