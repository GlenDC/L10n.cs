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

using L20n.Internal;

namespace L20n
{
	namespace Objects
	{
		public sealed class NegateExpression : L20nObject
		{	
			private readonly L20nObject m_Expression;
			
			public NegateExpression(L20nObject expression)
			{
				m_Expression = expression;
			}
			
			public override L20nObject Eval(Context ctx, params L20nObject[] argv)
			{
				var expression = m_Expression.Eval(ctx);
				var result = !(m_Expression.As<BooleanValue>().Value);
				
				return new BooleanValue(result);
			}
		}
	}
}
