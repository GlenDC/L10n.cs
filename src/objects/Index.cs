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

using L20n.Utils;
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
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Indeces.Length == 1) {
					return m_Indeces[0].Eval(ctx)
						.UnwrapAs<Identifier>().MapOrWarning(
							(index) => new Option<L20nObject>(index),
						    "something went wrong while evaluating the only index");
				}

				var indeces = new L20nObject[m_Indeces.Length];
				for (int i = 0; i < indeces.Length; ++i) {
					var index = m_Indeces[i].Eval(ctx).UnwrapAs<Identifier>();
					if(index.IsSet) {
						indeces[i] = index.Unwrap();
					}
					else {
						Internal.Logger.WarningFormat(
							"something went wrong while evaluating index #{0}", i);
						return L20nObject.None;
					}
				}

				var propertyExpression = new Objects.PropertyExpression(indeces);
				return propertyExpression.Eval(ctx);
			}
		}
	}
}
