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

using L20n.Internal;
using L20n.Exceptions;

namespace L20n
{
	namespace Objects
	{
		public sealed class StringValue : Primitive
		{	
			public string Value
			{
				get
				{
					if(m_Expressions.Length != 0) {
						throw new EvaluateException(
							String.Format(
								"can't return `{0}` as <string_value> has {1} expression(s)",
								m_Value, m_Expressions.Length));
					}

					return m_Value;
				}
			}

			private readonly string m_Value;
			private readonly L20nObject[] m_Expressions;
			
			public StringValue(string value, List<L20nObject> expressions)
			{
				m_Value = value;
				m_Expressions = expressions.ToArray();
			}
			
			public override L20nObject Eval(Context ctx, params L20nObject[] argv)
			{
				string[] expressions = new string[m_Expressions.Length];
				for (int i = 0; i < expressions.Length; ++i)
					expressions[i] = m_Expressions[i].Eval(ctx).As<Primitive>().ToString(ctx);
				var output = String.Format(m_Value, expressions);
				return new StringOutput(output);
			}
			
			public override string ToString(Context ctx, params L20nObject[] argv)
			{
				return Eval(ctx).As<Primitive>().ToString(ctx, argv);
			}
		}
	}
}
