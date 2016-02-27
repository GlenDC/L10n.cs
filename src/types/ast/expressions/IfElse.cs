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

namespace L20n
{
	namespace Types
	{
		namespace AST
		{
			namespace Expressions
			{
				public class IfElse : Expression
				{
					private readonly Expression m_Condition;
					private readonly Expression m_IfFalse;
					private readonly Expression m_IfTrue;
					
					public IfElse(Expression condition, Expression if_true, Expression if_false)
					{
						m_Condition = condition;
						m_IfTrue = if_true;
						m_IfFalse = if_false;
					}
					
					public override bool Evaluate(out Internal.Expression output)
					{
						throw new Exception("TODO");
					}
				}
			}
		}
	}
}

