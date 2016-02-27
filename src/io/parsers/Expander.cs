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
		namespace Parsers
		{	
			public class Expander
			{
				public static Types.AST.Expression Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					Types.AST.Expression expression;
					
					try {
						// skip opening tags
						stream.SkipString(2, "{{");
						// parse actual expression
						expression = Expression.Parse(stream);
						// skip closing tags
						stream.SkipString(2, "}}");

						return expression;
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing an <expander> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new IOException(msg, e);
					}
				}
				
				public static bool PeekAndParse(CharStream stream, out Types.AST.Expression expression)
				{
					if(stream.PeekNextRange(2) == "{{") {
						expression = Expander.Parse(stream);
						return true;
					}
					
					expression = null;
					return false;
				}
			}
		}
	}
}
