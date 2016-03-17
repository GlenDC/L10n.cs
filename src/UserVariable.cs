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

namespace L20nCore
{
	public sealed class UserVariable
	{
		public Utils.Option<Objects.L20nObject> Value {
			get;
			private set;
		}

		public UserVariable(Objects.L20nObject value)
		{
			Value = new Utils.Option<Objects.L20nObject>(value);
		}
		
		public UserVariable(Objects.DelegatedObject.Delegate callback)
		{
			Value = new Utils.Option<Objects.L20nObject>(
				new Objects.DelegatedObject(callback));
		}

		public static implicit operator UserVariable(string rhs)
		{
			return new UserVariable(new Objects.StringOutput(rhs));
		}

		public static implicit operator UserVariable(int rhs)
		{
			return new UserVariable(new Objects.Literal(rhs));
		}
		
		public static implicit operator UserVariable(bool rhs)
		{
			return new UserVariable(new Objects.BooleanValue(rhs));
		}

		public static implicit operator UserVariable(External.UserHashValue rhs)
		{
			var info = new External.InfoCollector();
			rhs.Collect(info);
			var hash = info.Collect().As<Objects.HashValue>().Unwrap();
			return new UserVariable(
				new Objects.Entity(new Utils.Option<Objects.L20nObject>(), false, hash));
		}
	}
}