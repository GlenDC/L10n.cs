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

using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		public sealed class Literal : Primitive
		{
			public int Value
			{
				get { return m_Value; }
				set { m_Value = value; }
			}
			
			private int m_Value;

			public Literal() {}

			public Literal(int value)
			{
				m_Value = value;
			}

			public override L20nObject Optimize()
			{
				return this;
			}

			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return this;
			}

			public override string ToString()
			{
				return m_Value.ToString();
			}

			public override string ToString(LocaleContext ctx, params L20nObject[] argv)
			{
				return this.ToString();
			}
		}
	}
}
