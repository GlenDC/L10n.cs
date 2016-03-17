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

			public StringValue(string value, L20nObject[] expressions)
			{
				m_Value = value;
				m_Expressions = expressions;
			}

			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Expressions.Length == 0)
					return new Option<L20nObject>(new StringOutput(m_Value));

				string[] expressions = new string[m_Expressions.Length];
				for (int i = 0; i < expressions.Length; ++i) {
					var e = m_Expressions[i].Eval(ctx);

					if(!e.IsSet) {
						return e;
					}

					Option<Identifier> id;
					while((id = e.UnwrapAs<Identifier>()).IsSet) {
						e = ctx.GetEntity(id.Unwrap().Value)
							.Map((entity) => entity.Eval(ctx));
					}

					var stringOutput =
						e.UnwrapAs<Primitive>().Map((o => o.ToString(ctx)));

					if(!stringOutput.IsSet) {
						return L20nObject.None;
					}

					expressions[i] = stringOutput.Unwrap();
				}

				var output = String.Format(m_Value, expressions);
				return new Option<L20nObject>(new StringOutput(output));
			}

			public override L20nObject Optimize()
			{
				// if we have no expressions, than we can already return with a result
				if(m_Expressions.Length == 0)
					return new StringOutput(m_Value);

				var values = new string[m_Expressions.Length];
				L20nObject expression;
				Primitive primitive;
				for(int i = 0; i < values.Length; ++i) {
					expression = m_Expressions[i].Optimize();
					primitive = expression.As<Primitive>().UnwrapOr(null);
					if(primitive == null) {
						return this; // we can't optimize this
					}

					if(primitive.As<StringValue>().IsSet) {
						return this;
					}

					values[i] = primitive.ToString();
				}

				var output = String.Format(m_Value, values);
				return new StringOutput(output);
			}

			public override Option<string> ToString(LocaleContext ctx, params L20nObject[] argv)
			{
				return Eval(ctx)
					.UnwrapAs<Primitive>().Map(
						(primitive) => primitive.ToString(ctx));
			}
		}
	}
}
