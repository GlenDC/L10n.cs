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
		namespace Parsers
		{
			namespace Expressions
			{
				public class Global
				{
					public static AST.INode Parse(CharStream stream)
					{
						stream.SkipCharacter('@');
						var identifier = Identifier.Parse(stream, false);
						return new AST.Global(identifier);
					}

					public static bool Peek(CharStream stream)
					{
						return stream.PeekNext() == '@';
					}
					
					public static bool PeekAndParse(
						CharStream stream, out AST.INode variable)
					{
						if (!Global.Peek(stream)) {
							variable = null;
							return false;
						}
						
						variable = Global.Parse(stream);
						return true;
					}
				}
			}
		}
	}
}
