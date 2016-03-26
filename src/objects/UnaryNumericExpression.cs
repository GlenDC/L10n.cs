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

using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		public abstract class UnaryNumericExpression : L20nObject
		{	
			private readonly L20nObject m_Expression;
			
			public UnaryNumericExpression(L20nObject expression)
			{
				m_Expression = expression;
			}

			public override L20nObject Optimize()
			{
				var literal = m_Expression.Optimize() as Literal;
				if (literal != null)
					return Operation(literal.Value);

				return this;
			}
			
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				var literal = m_Expression.Eval(ctx) as Literal;
				if(literal == null) {
					Logger.Warning("couldn't operate on non-valid literal evaluation");
					return literal;
				}

				return Operation(literal.Value);
			}
			
			protected abstract Literal Operation(int a);
		}
		
		public sealed class PositiveExpression : UnaryNumericExpression
		{
			public PositiveExpression(L20nObject e) : base(e) {}
			
			protected override Literal Operation(int a)
			{
				return new Literal(+a);
			}
		}
		
		public sealed class NegativeExpression : UnaryNumericExpression
		{
			public NegativeExpression(L20nObject e) : base(e) {}
			
			protected override Literal Operation(int a)
			{
				return new Literal(-a);
			}
		}
	}
}
