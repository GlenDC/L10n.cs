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

namespace L20n
{
	namespace Objects
	{
		public sealed class Entity : L20nObject
		{	
			private readonly Utils.Optional<L20nObject> m_Index;
			private readonly L20nObject m_Value;

			public Entity(Utils.Optional<L20nObject> index, L20nObject value)
			{
				m_Index = index;
				m_Value = value;
			}
			
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Index.IsSet) {
					var arguments = new List<L20nObject> (argv.Length + 1);
					var index = m_Index.ExpectAs<Index> ();
					arguments.Add (index.Eval (ctx));
					arguments.AddRange (argv);
					return m_Value.Eval (ctx, arguments.ToArray ());
				}

				return m_Value.Eval(ctx, argv);
			}
		}
	}
}
