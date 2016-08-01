// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
