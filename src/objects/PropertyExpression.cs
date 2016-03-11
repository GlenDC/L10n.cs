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
		public sealed class PropertyExpression : L20nObject
		{
			public L20nObject[] Identifiers
			{
				get { return m_Identifiers; }
			}

			private readonly L20nObject[] m_Identifiers;
			
			public PropertyExpression(L20nObject[] identifiers)
			{
				m_Identifiers = identifiers;
			}
			
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (argv.Length == 1 && argv[0] != null) {
					return argv[0].As<HashValue>().Eval(ctx, this);
				}
				
				var identifier = Identifiers[0].Eval(ctx).As<Identifier>().Value;
				var entity = ctx.GetEntity(identifier);
				return entity.Eval(ctx, new PropertyExpression(SliceIdentifiers(1)));
			}

			private L20nObject[] SliceIdentifiers(int start)
			{	
				// Return new array.
				var res = new L20nObject[m_Identifiers.Length - start];
				for (int i = start; i < m_Identifiers.Length; i++)
					res[i] = m_Identifiers[i];

				return res;
			}
		}
	}
}
