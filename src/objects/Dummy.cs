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
		/// The dummy object is used by the Database to indicate that an L20nObject that
		/// if it is marked as private it should not translate, as it shouldn't be accessed.
		/// </summary>
		/// <remarks>
		/// I'm not a fan of the approach of this solution to the Private/Public problem,
		/// but it does work within the current System.
		/// 
		/// If you however read this and read through the other parts of this codebase
		/// and came up with a better solution, feel free to mail me at contact@glendc.com.
		/// </remarks>
		public sealed class Dummy : L20nObject
		{	
			public Dummy()
			{
			}

			// this method should never be called as it is... a dummy object
			public override L20nObject Optimize()
			{
				throw new NotImplementedException();
			}

			// this method should never be called as it is... a dummy object
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				throw new NotImplementedException();
			}
		}
	}
}
