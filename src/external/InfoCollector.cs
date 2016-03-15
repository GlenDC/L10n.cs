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

using L20n.Objects;
using L20n.Utils;

namespace L20n
{
	namespace External
	{
		public sealed class InfoCollector
		{
			public bool IsHash
			{
				get { return !m_SimpleVariable.IsSet; }
			}
			private readonly Dictionary<string, L20nObject> m_Info;
			private readonly Option<L20nObject> m_SimpleVariable;
	
			public InfoCollector()
			{
				m_Info = new Dictionary<string, L20nObject>();
				m_SimpleVariable = new Option<L20nObject>();
			}
			
			public void Set(int value)
			{
				SetSimpleObject(new Literal(value));
			}
			
			public void Set(string value)
			{
				SetSimpleObject(new StringOutput(value));
			}

			public void Add(string name, int value)
			{
				AddObject(name, new Literal(value));
			}
			
			public void Add(string name, string value)
			{
				AddObject(name, new StringOutput(value));
			}
			
			public void Add(string name, bool value)
			{
				AddObject(name, new BooleanValue(value));
			}
			
			public void Add(string name, IVariable value)
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

			public L20nObject Collect()
			{
				if(m_SimpleVariable.IsSet)
					return m_SimpleVariable.Unwrap();

				return new HashValue(m_Info, null);
			}

			public void Clear()
			{
				m_Info.Clear();
				m_SimpleVariable.Unset();
			}

			public void SetSimpleObject(L20nObject value)
			{
				if (m_SimpleVariable.IsSet) {
					Internal.Logger.Warning(
						"information has already a simple variable and it will be overriden");
				}

				if (m_Info.Count != 0) {
					Internal.Logger.Warning(
						"information has already children added, " +
						"which will be ignored because of the simple variable you're adding now");
				}

				m_SimpleVariable.Set(value);
			}

			private void AddObject(string name, L20nObject value)
			{
				if (m_SimpleVariable.IsSet) {
					Internal.Logger.Warning(
						"external variable is marked as a simple variable, " +
						"and thus can not have children");
					return;
				}

				if (m_Info.ContainsKey(name)) {
					Internal.Logger.WarningFormat(
						"information with the name {0} will be overriden", name);
				}

				m_Info.Add(name, value);
			}
		}
	}
}
