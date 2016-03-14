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

using L20n.Exceptions;
using L20n.Internal;
using L20n.Utils;

namespace L20n
{
	namespace Objects
	{
		public abstract class L20nObject
		{
			public abstract Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv);

			private static Option<L20nObject> m_None = new Option<L20nObject>();
			public static Option<L20nObject> None
			{
				get { return m_None; }
			}

			public Option<T> As<T>() where T: L20nObject
			{
				return new Option<T>(this as T);
			}
		}
	}
}
