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
			public class Macro
			{
				public static void Parse(
					CharStream stream,
					string identifier, Internal.LocaleContext.Builder builder)
				{
					var startingPos = stream.Position;
					
					try
					{
						var macroAST = new AST.Macro(identifier);
						
						stream.SkipCharacter('(');
						WhiteSpace.Parse(stream, true);

						// variables are optional,
						// but we do have them, we need at least one (duh)
						if (Expressions.Variable.Peek(stream))
						{
							macroAST.AddParameter(Macro.ParseVariable(stream));

							// more than 1 is possible as well
							while (stream.SkipIfPossible(','))
							{
								WhiteSpace.Parse(stream, true);
								macroAST.AddParameter(Macro.ParseVariable(stream));
							}
						}

						stream.SkipCharacter(')');
						WhiteSpace.Parse(stream, false);

						stream.SkipCharacter('{');
						WhiteSpace.Parse(stream, true);

						// Parse the Actual Macro Expression
						macroAST.SetExpression(Expression.Parse(stream));

						WhiteSpace.Parse(stream, true);
						stream.SkipCharacter('}');
						WhiteSpace.Parse(stream, true);
						stream.SkipCharacter('>');

						// return the fully parsed macro
						try
						{
							var macro = (Objects.Macro)macroAST.Eval();
							builder.AddMacro(identifier, macro);
						} catch (Exception e)
						{
							throw new Exceptions.EvaluateException(
								String.Format("couldn't evaluate `{0}`", macroAST.Display()),
								e);
						}
					} catch (Exception e)
					{
						string msg = String.Format(
							"something went wrong parsing a <macro> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new Exceptions.ParseException(msg, e);
					}
				}

				public static bool PeekAndParse(
					CharStream stream,
					string identifier, Internal.LocaleContext.Builder builder)
				{
					if (stream.PeekNext() != '(')
					{
						return false;
					}

					Macro.Parse(stream, identifier, builder);
					return true;
				}
				
				private static string ParseVariable(CharStream stream)
				{
					var variable = Expressions.Variable.Parse(stream) as AST.Variable;
					WhiteSpace.Parse(stream, true);
					return variable.Value;
				}
			}
		}
	}
}
