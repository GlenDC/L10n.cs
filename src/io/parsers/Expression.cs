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
			/// <summary>
			/// The combinator parser used to parse any of the possible expreesions.
			/// This bubbles down all the way to the most primitive expression if needed.
			/// </summary>
			/// <remarks>
			/// An expression is the same as a conditional expression,
			/// according to the L20n spec it can ONLY ever be a conditional expression
			/// so there is no need to make 2 seperate parsers for it
			/// </remarks>
			public static class Expression
			{
				public static AST.INode Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try
					{
						var condition = Expressions.Logic.Parse(stream);

						// check if we have an IfElse case or simply a logical expression
						string s;
						if (stream.ReadReg(@"\s*\?\s*", out s))
						{
							var first = Expression.Parse(stream);
							WhiteSpace.Parse(stream, true);
							stream.SkipCharacter(':');
							WhiteSpace.Parse(stream, true);
							var second = Expression.Parse(stream);
							return new AST.Conditional(condition, first, second);
						} else
						{ // it's simply a logical expression
							return condition;
						}
					} catch (Exception e)
					{
						string msg = String.Format(
							"something went wrong parsing an <expression> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new Exceptions.ParseException(msg, e);
					}
				}
			}
		}
	}
}
