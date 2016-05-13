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
		/// <summary>
		/// The <see cref="L20nCore.Objects.StringValue"/> class is a
		/// <see cref="L20nCore.Objects.Primitive"/> L20n Object Type
		/// and can therefore be evaluated to a <see cref="L20nCore.Objects.StringOutput"/> value.
		///
		/// It consists of a format string and <c>x</c> amount of expressions that will make up
		/// the final values that will be used to evaluate the format string into its final output value.
		/// </summary>
		public sealed class StringValue : Primitive
		{
			private readonly string m_Value;
			private readonly L20nObject[] m_Expressions;

			// objects to stay responsible with the GC
			private readonly string[] m_EvaluatedExpressions;
			private readonly StringOutput m_Output;

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.StringValue"/> class
			/// with the format string (<c>value</c>) and the <c>expressions</c>
			/// given by the callee of this constructor.
			/// </summary>
			public StringValue(string value, L20nObject[] expressions)
			{
				m_Value = value;
				m_Expressions = expressions;

				m_EvaluatedExpressions = new string[expressions.Length];
				m_Output = new StringOutput();
			}

			/// <summary>
			/// Evaluate all the expressions contained in this <see cref="L20nCore.Objects.StringValue"/>
			/// and use the results of these evaluations as the variables used in the evaluation
			/// of the format string to compute the final result value as a <see cref="L20nCore.Objects.StringOutput"/>.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				// Evaluate each expression and store them in the cached list
				for (int i = 0; i < m_EvaluatedExpressions.Length; ++i)
				{
					var e = m_Expressions [i].Eval(ctx);

					// if one cannot evaluate, we can't know the real final result
					// and thus we return early
					if (e == null)
					{
						return null;
					}

					// Identifiers get evaluated until we get a non-reference value
					Identifier id;
					Entity entity;
					while ((id = e as Identifier) != null)
					{
						entity = ctx.GetEntity(id.Value);
						if (entity != null)
							e = entity.Eval(ctx);
					}

					var primitive = e as Primitive;

					// in the end we expect to be left with a primitive value,
					// if not there is something wrong in the global evaluation logic.
					if (primitive == null)
					{
						Logger.WarningFormat("<StringValue>: couldn't evaluate expression #{0}", i);
						return null;
					}

					// we expect a primitive value so that we can turn it into a string
					var stringOutput = primitive.ToString(ctx);
					if (stringOutput == null)
					{
						Logger.WarningFormat("<StringValue>: couldn't evaluate expression #{0} to <StringOutput>", i);
						return null;
					}

					m_EvaluatedExpressions [i] = stringOutput;
				}

				// we update the cached StringOutput value and return it as
				// the result of this evaluation.
				m_Output.Value = FormatString(m_Value, m_EvaluatedExpressions);
				return m_Output;
			}

			public override L20nObject Optimize()
			{
				// if we have no expressions, than we can already return with a result
				if (m_Expressions.Length == 0)
					return new StringOutput(m_Value);

				// We optimize the expressions we can optimize
				Primitive primitive;
				bool fullyOptimized = true;
				for (int i = 0; i < m_EvaluatedExpressions.Length; ++i)
				{
					m_Expressions [i] = m_Expressions [i].Optimize();
					primitive = m_Expressions [i] as Primitive;
					if (primitive == null || (primitive as StringValue) != null)
					{
						fullyOptimized = false;
					} else if (fullyOptimized)
					{
						m_EvaluatedExpressions [i] = primitive.ToString(null);
					}
				}

				// if all expressions have been optimized,
				// we can return a stringOutput
				if (fullyOptimized)
				{
					m_Output.Value = FormatString(m_Value, m_EvaluatedExpressions);
					return m_Output;
				}

				// else we return this object,
				// which might be partially optimized.
				return this;
			}

			/// <summary>
			/// Evaluates (this.Eval) this <see cref="L20nCore.Objects.StringValue"/> and returns
			/// the string value of the computed StringOutput output.
			/// Returns <c>null</c> in case the call to <c>this.Eval</c> resulted in <c>null</c> as well.
			/// </summary>
			public override string ToString(LocaleContext ctx, params L20nObject[] argv)
			{
				var str = Eval(ctx) as StringOutput;
				return str != null ? str.Value : null;
			}

			/// <summary>
			/// A Simple utility function to format the string with our custom formatter based
			/// on the expanders that are usd within L20n StirngValues.
			/// </summary>
			/// <remarks>
			/// This is quite a lot of string allocations for what it is though,
			/// we might want to cache those strings or just try to prevent them instead.
			/// </remarks>
			private string FormatString(string format, string[] argv)
			{
				for (int i = 0; i < argv.Length; ++i)
					format = format.Replace(
						String.Format("{{ {1}{0}{1} }}", i, IO.AST.StringValue.DummyExpressionCharacter),
						argv [i]);
				return format;
			}
		}
	}
}
