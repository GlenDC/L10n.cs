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

using L20n.Internal;

namespace L20n
{
	namespace Objects
	{
		public sealed class Variable : L20nObject
		{	
			public string Identifier
			{
				get { return m_Identifier; }
			}

			private readonly string m_Identifier;
			
			public Variable(string identifier)
			{
				m_Identifier = identifier;
			}
			
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				// TODO
				return null;
				//return ctx.GetVariable(m_Identifier);
			}
		}
	}
}
