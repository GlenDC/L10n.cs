// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
