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
			public class Index : INode<List<L20n.Types.Internal.Expression>>
			{
				private List<Expression> m_Expressions;

				public Index()
				{
					m_Expressions = new List<Expression>();
				}

				public void AddExpression(Expression expression)
				{
					m_Expressions.Add(expression);
				}

				public bool Evaluate(out List<L20n.Types.Internal.Expression> output)
				{
					var list = new List<L20n.Types.Internal.Expression>(m_Expressions.Count);
					L20n.Types.Internal.Expression expression;

					for(int i = 0; i < m_Expressions.Count; ++i) {
						if(m_Expressions[i].Evaluate(out expression))
							list.Add(expression);
					}

					if (list.Count > 0) {
						output = list;
						return true;
					}

					output = null;
					return false;
				}
			}
		}
	}
}

