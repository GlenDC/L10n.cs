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

using L20nCore.Exceptions;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Internal
	{
		public sealed class Cache
		{
			private Dictionary<string, string> m_Cache;
			private Option<string> m_None;

			public Cache()
			{
				m_Cache = new Dictionary<string, string>();
				m_None = new Option<string>();
			}

			public void Clear()
			{
				m_Cache.Clear();
			}

			public Option<string> TryGet(string id)
			{
				string output;
				if(m_Cache.TryGetValue(id, out output))
					return new Option<string>(output);
				return m_None;
			}

			public void Set(string id, string value)
			{
				m_Cache.Add(id, value);
			}
		}
	}
}
