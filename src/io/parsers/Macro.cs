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
			public class Macro
			{
				public static void Parse(
					CharStream stream,
					string identifier, L20n.Internal.ContextBuilder builder)
				{
					var startingPos = stream.Position;
					
					try {
						stream.SkipCharacter('(');
						WhiteSpace.Parse(stream, true);

						List<string> variables = null;

						// variables are optional,
						// but we do have them, we need at least one (duh)
						if (Expressions.Variable.Peek (stream)) {
							variables = new List<string>();
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
						builder.AddMacro(identifier,
							new L20n.Objects.Macro(expression, variables));
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing a <macro> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new L20n.Exceptions.ParseException(msg, e);
					}
				}

				public static bool PeekAndParse(
					CharStream stream,
					string identifier, Internal.ContextBuilder builder)
				{
					if (stream.PeekNext () != '(') {
						return false;
					}

					Macro.Parse(stream, identifier, builder);
					return true;
				}
				
				private static string ParseVariable(CharStream stream)
				{
					var variable = Expressions.Variable.Parse(stream);
					WhiteSpace.Parse(stream, true);
					return variable.As<L20n.Objects.Variable>().Identifier;
				}
			}
		}
	}
}
