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
		/// <summary>
		/// A primitive L20nObject is any L20nObject type that can be
		/// directly translated to a <see cref="System.String"/>. 
		/// </summary>
		public abstract class Primitive : L20nObject
		{
			/// <summary>
			/// Compute a string based on the <see cref="L20nCore.Objects.Primitive"/> object
			/// and its content.
			/// </summary>
			public abstract string ToString(LocaleContext ctx, params L20nObject[] argv);
		}
	}
}
