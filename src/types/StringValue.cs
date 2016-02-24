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
	namespace Types
	{
		public class StringValue : Types.Value
		{
			private string m_Value;
			private	List<Expression> m_Expressions;
			private L20n.IO.Parsers.Quote.Info m_QuoteInfo;

			public StringValue(L20n.IO.Parsers.Quote.Info info)
			{
				m_Value = "";
				m_Expressions = new List<Expression>();
				m_QuoteInfo = info;
			}

			public void appendChar(char c)
			{
				m_Value += c;
			}

			public void appendString(string s)
			{
				m_Value += s;
			}

			public void appendExpression(Expression e)
			{
				appendString(String.Format ("{{0}}", m_Expressions.Count));
				m_Expressions.Add(e);
			}

			public override string ToString()
			{
				string e =
					m_Expressions.Count == 0
						? m_Value
						: String.Format (m_Value, m_Expressions);

				return String.Format("{0}{1}{0}", m_QuoteInfo.ToString(), e);
			}
		}
	}
}

