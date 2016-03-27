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
		public sealed class Index : L20nObject
		{
			private readonly L20nObject[] m_Indeces;
			private L20nObject[] m_EvaluatedIndeces;
			
			public Index(L20nObject[] indeces)
			{
				m_Indeces = indeces;
				m_EvaluatedIndeces = new L20nObject[indeces.Length];
			}

			public override L20nObject Optimize()
			{
				// relies on ctx use, so no way to optimize this
				return this;
			}
			
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Indeces.Length == 1)
				{
					var index = m_Indeces [0].Eval(ctx);

					var identifier = index as Identifier;
					if (identifier != null)
						return index;

					var stringOutput = index as StringOutput;
					if (stringOutput != null)
						return new Identifier(stringOutput.Value);

					Logger.Warning("something went wrong while evaluating the only index");
					return null;
				}

				for (int i = 0; i < m_EvaluatedIndeces.Length; ++i)
				{
					var index = m_Indeces [i].Eval(ctx);
					var identifier = index as Identifier;
					if (identifier == null)
					{
						var output = index as StringOutput;
						if (output != null)
							identifier = new Identifier(output.Value);
					}

					if (identifier != null)
					{
						m_EvaluatedIndeces [i] = identifier;
					} else
					{
						Internal.Logger.WarningFormat(
							"something went wrong while evaluating index #{0}", i);
						return null;
					}
				}

				return new PropertyExpression(m_EvaluatedIndeces);
			}
		}
	}
}
