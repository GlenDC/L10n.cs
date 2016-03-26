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

using L20nCore.Utils;
using L20nCore.Internal;
using L20nCore.Exceptions;

namespace L20nCore
{
	namespace Objects
	{
		public sealed class StringValue : Primitive
		{
			private readonly string m_Value;
			private readonly L20nObject[] m_Expressions;

			// objects to stay responsible with the GC
			private readonly string[] m_EvaluatedExpressions;
			private readonly StringOutput m_Output;

			public StringValue(string value, L20nObject[] expressions)
			{
				m_Value = value;
				m_Expressions = expressions;

				m_EvaluatedExpressions = new string[expressions.Length];
				m_Output = new StringOutput();
			}

			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				for (int i = 0; i < m_EvaluatedExpressions.Length; ++i) {
					var e = m_Expressions[i].Eval(ctx);

					if(e == null) {
						return null;
					}

					Identifier id;
					Entity entity;
					while((id = e as Identifier) != null) {
						entity = ctx.GetEntity(id.Value);
						if(entity != null)
							e = entity.Eval(ctx);
					}

					var primitive = e as Primitive;

					if(primitive == null) {
						Logger.WarningFormat("<StringValue>: couldn't evaluate expression #{0}", i);
						return null;
					}

					var stringOutput = primitive.ToString(ctx);
					if(stringOutput == null) {
						Logger.WarningFormat("<StringValue>: couldn't evaluate expression #{0} to <StringOutput>", i);
						return null;
					}

					m_EvaluatedExpressions[i] = stringOutput;
				}

				m_Output.Value = String.Format(m_Value, m_EvaluatedExpressions);
				return m_Output;
			}

			public override L20nObject Optimize()
			{
				// if we have no expressions, than we can already return with a result
				if(m_Expressions.Length == 0)
					return new StringOutput(m_Value);

				var values = new string[m_Expressions.Length];
				Primitive primitive;
				for(int i = 0; i < values.Length; ++i) {
					primitive = m_Expressions[i].Optimize() as Primitive;
					if(primitive == null) {
						return this; // we can't optimize this
					}

					// clearly this is a primitive that can't be optimized, so let's return
					if((primitive as StringValue) != null) {
						return this;
					}

					values[i] = primitive.ToString();
				}

				m_Output.Value = String.Format(m_Value, values);
				return m_Output;
			}

			public override string ToString(LocaleContext ctx, params L20nObject[] argv)
			{
				var str = Eval(ctx) as StringOutput;
				return str != null ? str.Value : null;
			}
		}
	}
}
