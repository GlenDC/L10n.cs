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

namespace L20n
{
	namespace External
	{
		/// <summary>
		/// A simple interface that allows you to use objects
		/// as external variables.
		/// </summary>
		/// <remarks>
		/// In case the stuff you write is flexible,
		/// make sure that you provide your own checks to not use stuff that you don't provide.
		/// </remarks>
		public abstract class UserHashValue
		{
			abstract public void Collect(InfoCollector info);
		}
	}
}
