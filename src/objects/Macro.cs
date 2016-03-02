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
		public sealed class Macro : L20nObject
		{
			private readonly string[] m_Parameters;
			private readonly L20nObject m_Expression;

			public Macro(L20nObject expression, List<string> parameters)
			{
				m_Parameters = parameters.ToArray();
				m_Expression = expression;
			}

			public override L20nObject Eval(Context ctx, params L20nObject[] argv)
			{
				if (m_Parameters.Length != argv.Length) {
					var msg = String.Format(
						"<macro> expects {0} parameters, received {1}",
						m_Parameters.Length, argv.Length);
					throw new EvaluateException(msg);
				}

				// TODO bind variables to identifiers -> TO CTX

				var obj = m_Expression.Eval(ctx);

				// unbind the variables

				return obj;
			}
		}
	}
}
