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

using L20nCore.Objects;
using L20nCore.Utils;

namespace L20nCore
{
	namespace External
	{
		public sealed class InfoCollector
		{
			private readonly Dictionary<string, L20nObject> m_Info;
	
			public InfoCollector()
			{
				m_Info = new Dictionary<string, L20nObject>();
			}

			public void Add(string name, int value)
			{
				AddObject(name, new Literal(value));
			}
			
			public void Add(string name, string value)
			{
				AddObject(name, new StringOutput(value));
			}

			public void Add(string name, Objects.DelegatedLiteral.Delegate callback)
			{
				AddObject(name, new Objects.DelegatedLiteral(callback));
			}
			
			public void Add(string name, Objects.DelegatedString.Delegate callback)
			{
				AddObject(name, new Objects.DelegatedString(callback));
			}
			
			public void Add(string name, IHashValue value)
			{
				// Prepare the collector
				var info = new InfoCollector();

				// Collect the given value
				value.Collect(info);
				
				if (m_Info.Count == 0) {
					Internal.Logger.Warning(
						"can't add an external variable that has no information exposed," +
						" please add information by calling `Info.Add(...)`");
					return;
				}

				// Add it as a child to the current object
				AddObject(name, info.Collect());
			}

			public HashValue Collect()
			{
				return new HashValue(m_Info, null);
			}

			public void Clear()
			{
				m_Info.Clear();
			}

			private void AddObject(string name, L20nObject value)
			{
				if (m_Info.ContainsKey(name)) {
					Internal.Logger.WarningFormat(
						"information with the name {0} will be overriden", name);
				}

				m_Info.Add(name, value);
			}
		}
	}
}
