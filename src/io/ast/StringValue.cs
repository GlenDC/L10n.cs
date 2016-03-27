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
		namespace AST
		{
			public sealed class StringValue : INode
			{
				private readonly Parsers.Quote.Info m_Quote;
				private string m_Value;
				private List<INode> m_Expressions;

				public StringValue(Parsers.Quote.Info quote)
				{
					m_Quote = quote;
					m_Value = "";
					m_Expressions = new List<INode>();
				}

				public void appendChar(char c)
				{
					m_Value += c;
				}

				public void appendString(string s)
				{
					m_Value += s;
				}

				public void appendExpression(INode expression)
				{
					appendString("{" + m_Expressions.Count + "}");
					m_Expressions.Add(expression);
				}

				public Objects.L20nObject Eval()
				{
					var expressions = new Objects.L20nObject[m_Expressions.Count];
					for (int i = 0; i < expressions.Length; ++i)
						expressions [i] = m_Expressions [i].Eval();
					return new Objects.StringValue(m_Value, expressions).Optimize();
				}

				public string Display()
				{
					var expressions = new string[m_Expressions.Count];
					for (int i = 0; i < expressions.Length; ++i)
					{
						expressions [i] = m_Expressions [i].Display();
					}

					var str = String.Format(m_Value, expressions);
					return String.Format("{0}{1}{0}", m_Quote.ToString(), str);
				}
			}
		}
	}
}
