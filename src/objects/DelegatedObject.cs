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

using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		public sealed class DelegatedObject<T> : L20nObject
			where T: L20nObject
		{
			private readonly Delegate m_Callback;
			
			public DelegatedObject(Delegate callback)
			{
				m_Callback = callback;
			}

			public override L20nObject Optimize()
			{
				return this;
			}
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return new Option<L20nObject>(m_Callback());
			}
			
			public delegate T Delegate();
		}

		public sealed class DelegatedLiteral : L20nObject
		{
			private readonly Delegate m_Callback;
			
			public DelegatedLiteral(Delegate callback)
			{
				m_Callback = callback;
			}
			
			public override L20nObject Optimize()
			{
				return this;
			}
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return new Option<L20nObject>(new Literal(m_Callback()));
			}
			
			public delegate int Delegate();
		}

		public sealed class DelegatedString : L20nObject
		{
			private readonly Delegate m_Callback;
			
			public DelegatedString(Delegate callback)
			{
				m_Callback = callback;
			}
			
			public override L20nObject Optimize()
			{
				return this;
			}
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return new Option<L20nObject>(new StringOutput(m_Callback()));
			}
			
			public delegate string Delegate();
		}
	}
}
