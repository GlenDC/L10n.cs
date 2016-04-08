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
		/// The <see cref="L20nCore.Objects.IdentifierExpression"/> class represents a reference
		/// to another L20nObject in the current or default <see cref="L20nCore.Internal.LocaleContext"/>.
		/// </summary>
		public sealed class IdentifierExpression : L20nObject
		{	
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Identifier"/> class,
			/// with the given <c>value</c> used to look up the L20nObject instance.
			/// </summary>
			public IdentifierExpression(string identifier)
			{
				m_Identifier = identifier;
			}

			/// <summary>
			/// <see cref="L20nCore.Objects.Identifier"/> can't be optimized
			/// and will instead return this instance.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// Tries to get the <see cref="L20nCore.Objects.Entity"/> instance based on the stored reference (name) 
			/// and will return the evaluation result of the looked up object if possible.
			/// Returns <c>null</c> in case the object could not be found or the evaluation
			/// of the looked up object returned <c>null</c> itself.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				var entity = ctx.GetEntity(m_Identifier);
				if (entity == null)
				{
					Logger.WarningFormat("couldn't find an entity with key {0}", m_Identifier);
					return entity;
				}

				return entity.Eval(ctx, argv);
			}

			private readonly string m_Identifier;
		}
	}
}
