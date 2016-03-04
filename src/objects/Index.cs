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
		public sealed class Index : L20nObject
		{
			private readonly L20nObject[] m_Indeces;
			
			public Index(L20nObject[] indeces)
			{
				m_Indeces = indeces;
			}
			
			public override L20nObject Eval(Context ctx, params L20nObject[] argv)
			{
				if (m_Indeces.Length == 1) {
					var index = m_Indeces[0].Eval(ctx).As<StringOutput>().Value;
					var identifier = new L20n.Objects.IdentifierExpression(index);
					return identifier.Eval(ctx);
				}

				var indeces = new L20nObject[m_Indeces.Length];
				for(int i = 0; i < indeces.Length; ++i)
					indeces[i] = m_Indeces[i].Eval(ctx);
				var propertyExpression = new L20n.Objects.PropertyExpression(indeces);
				return propertyExpression.Eval(ctx);
			}
		}
	}
}
