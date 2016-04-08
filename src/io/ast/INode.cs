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

namespace L20nCore
{
	namespace IO
	{
		namespace AST
		{
			/// <summary>
			/// The interface for all AST Object Types used in the AST of L20n.
			/// </summary>
			public interface INode
			{
				/// <summary>
				/// Evaluates the AST Object and returns the final Object,
				/// on top of that it will be the most optimized version of that object,
				/// meaning that the parser doesn't necessarily define the final type of the
				/// returning <see cref="L20nCore.Objects.L20nObject"/>.
				/// </summary>
				Objects.L20nObject Eval();

				/// <summary>
				/// Displays the expression in a string version.
				/// This is purely used for debugging reasons, and has no real
				/// use in a production environment.
				/// </summary>
				string Display();
			}
		}
	}
}
