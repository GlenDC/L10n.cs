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
			/// <summary>
			/// The AST representation for a StringValue, meaning that it is a string
			/// with optional expressions. It wil be optimized to a StringOutput in case there are no expressions.
			/// More Information: <see cref="L20nCore.IO.Parsers.StringValue"/> 
			/// </summary>
			public sealed class StringValue : INode
			{
				public char LastCharacter
				{
					get { return m_Value.Length == 0  ? '\0' : m_Value [m_Value.Length - 1]; }
				}

				/// <summary>
				/// Used to prevent any potential name clashes with escaped expressions in a StringValue
				/// </summary>
				public const char DummyExpressionCharacter = (char)7;
				/// <summary>
				/// Used to only add spaces for newline characters where spaces are required,
				/// if no spaces are present, no spaces will be added. Which should only be
				/// in languages like Chinese, or simply unneeded multiline strings
				/// (which should be corrected by the translator)
				/// </summary>
				public const char DummyNewlineWhitespaceCharacter = (char)6;

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
					// we add this rediculous character (255), in order to avoid
					// the situation where the user might want to have a similar
					// pattern in their StringValue, for some bizar reason.
					appendString(String.Format("{{ {1}{0}{1} }}", m_Expressions.Count, DummyExpressionCharacter));
					m_Expressions.Add(expression);
				}

				public Objects.L20nObject Eval()
				{
					var expressions = new Objects.L20nObject[m_Expressions.Count];
					for (int i = 0; i < expressions.Length; ++i)
						expressions [i] = m_Expressions [i].Eval();

					var value = m_Value.Trim();
					value = value.Replace(
						DummyNewlineWhitespaceCharacter.ToString(),
						value.Contains(" ") ? " " : "");

					return new Objects.StringValue(value, expressions).Optimize();
				}

				public string Display()
				{
					var expressions = new string[m_Expressions.Count];
					for (int i = 0; i < expressions.Length; ++i)
						expressions [i] = m_Expressions [i].Display();

					var value = m_Value.Trim();
					value = value.Replace(
						DummyNewlineWhitespaceCharacter.ToString(),
						value.Contains(" ") ? " " : "");

					var str = String.Format(value, expressions);
					return String.Format("{0}{1}{0}", m_Quote.ToString(), str);
				}
				
				private readonly Parsers.Quote.Info m_Quote;
				private string m_Value;
				private List<INode> m_Expressions;
			}
		}
	}
}
