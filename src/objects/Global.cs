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
		/// The <see cref="L20nCore.Objects.Global"/> class represents a reference
		/// to a global L20nObject.
		/// </summary>
		public sealed class Global : L20nObject
		{	
			/// <summary>
			/// Returns the reference, the name of the global L20nObject.
			/// </summary>
			public string Identifier
			{
				get { return m_Identifier; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Global"/> class,
			/// with the given <c>value</c> used to look the L20nObject global.
			/// </summary>
			public Global(string identifier)
			{
				m_Identifier = identifier;
			}

			public override L20nObject Optimize()
			{
				return this;
			}

			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				var global = ctx.GetGlobal(m_Identifier);
				if (global == null)
				{
					Logger.WarningFormat("couldn't find global with key {0}", m_Identifier);
					return global;
				}

				return global.Eval(ctx, argv);
			}
			
			private readonly string m_Identifier;
		}
	}
}
