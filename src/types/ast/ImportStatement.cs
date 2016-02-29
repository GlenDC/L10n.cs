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
	namespace Types
	{
		namespace AST
		{
			public sealed class ImportStatement : Entry
			{
				private readonly Types.AST.Expression m_Expression;
				private readonly string m_Root;

				public ImportStatement(Types.AST.Expression expression, string root)
				{
					m_Expression = expression;
					m_Root = root;
				}

				public override bool Evaluate (out List<L20n.Types.Entity> output)
				{
					var list = new List<L20n.Types.Entity>();

					Internal.Expression result;
					if(m_Expression.Evaluate(out result)) {
						var str = result as Internal.Expressions.StringValue;

						if(str == null) {
							string msg = String.Format(
								"an <expression> within an <import_statement> should be a {0}, got {1}",
								typeof(Internal.Expressions.StringValue),
								result.GetType());
							throw new IOException(msg);
						}
						
						string path = Path.Combine(m_Root, str.ToString());
						Console.WriteLine(path);
						if(IO.LocalizbleObjectsList.Parse(path, list)) {
							if(list.Count > 0) {
								output = list;
								return true;
							}
						}
					}

					output = null;
					return false;
				}
			}
		}
	}
}
