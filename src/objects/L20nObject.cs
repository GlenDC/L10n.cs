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

using L20nCore.Exceptions;
using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// The base class of all the different objects that make up the
		/// different blocks of the L20n language.
		/// </summary>
		public abstract class L20nObject
		{
			/// <summary>
			/// The optimize function allows the object to provide an optional
			/// optimize step which will be run when the object is created based on its AST counterpart.
			/// It's optional because an object could also simply return itself.
			/// </summary>
			public abstract L20nObject Optimize();

			/// <summary>
			/// The <c>Eval</c> methods returns the result of the object and its content
			/// with optional access to the current <see cref="L20nCore.Internal.LocaleContext"/>
			/// and optional parameters given by the callee (the object owner, in most cases another L20nObject type).
			/// </summary>
			public abstract L20nObject Eval(LocaleContext ctx, params L20nObject[] argv);
		}
	}
}
