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
		namespace AST
		{
			public class StringValue : Value
			{
				private string m_Value;
				private	List<Expression> m_Expressions;

				public StringValue()
				{
					m_Value = "";
					m_Expressions = new List<Expression>();
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

				public override bool Evaluate(out Internal.Expressions.Value output)
				{
					if (m_Value != "") {
						output = new Internal.Expressions.StringValue(m_Value);
						return true;
					}

					output = null;
					return false;
				}
			}
		}
	}
}

