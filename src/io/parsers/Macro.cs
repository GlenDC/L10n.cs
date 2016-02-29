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
			public class Macro
			{
				public static Types.AST.Entry Parse(CharStream stream, string identifier)
				{
					var startingPos = stream.Position;
					
					try {
						stream.SkipCharacter('(');
						WhiteSpace.Parse(stream, true);

						List<Types.Internal.Expressions.Variable> variables = null;

						// variables are optional,
						// but we do have them, we need at least one (duh)
						if (Expressions.Variable.Peek (stream)) {
							variables = new List<Types.Internal.Expressions.Variable>();
							variables.Add(Macro.ParseVariable(stream));

							// more than 1 is possible as well
							while(stream.SkipIfPossible(',')) {
								WhiteSpace.Parse(stream, true);
								variables.Add(Macro.ParseVariable(stream));
							}
						}

						stream.SkipCharacter(')');
						WhiteSpace.Parse(stream, false);

						stream.SkipCharacter('{');
						WhiteSpace.Parse(stream, true);

						// Parse the Actual Macro Expression
						var expression = Expression.Parse(stream);

						WhiteSpace.Parse(stream, true);
						stream.SkipCharacter('}');
						WhiteSpace.Parse(stream, true);
						stream.SkipCharacter('>');

						// return the fully parsed macro
						return new L20n.Types.AST.Macro(variables, expression);
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing a <macro> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new IOException(msg, e);
					}
				}

				public static bool PeekAndParse(CharStream stream, string identifier, out Types.AST.Entry macro)
				{
					if (stream.PeekNext () != '(') {
						macro = null;
						return false;
					}

					macro = Macro.Parse(stream, identifier);
					return true;
				}
				
				private static Types.Internal.Expressions.Variable ParseVariable(CharStream stream)
				{
					var variable = Expressions.Variable.Parse (stream)
						as Types.Internal.Expressions.Variable;
					WhiteSpace.Parse(stream, true);
					return variable;
				}
			}
		}
	}
}
