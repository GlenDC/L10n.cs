// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Utils;
using L20nCore.Common.Exceptions;

using L20nCore.L20n.Internal;

namespace L20nCore
{
	namespace L20n
	{
		namespace Objects
		{
			/// <summary>
			/// The base class of all the different objects that make up the
			/// different blocks of the L20n language.
			/// </summary>
			public abstract class FTLObject
			{
				public static readonly FTLObject Nop = null;

				/// <summary>
				/// The optimize function allows the object to provide an optional
				/// optimize step which will be run when the object is created based on its AST counterpart.
				/// It's optional because an object could also simply return itself.
				/// </summary>
				public abstract FTLObject Optimize();
				
				/// <summary>
				/// The <c>Eval</c> methods returns the result of the object and its content
				/// with optional access to the current <see cref="L20nCore.L20n.Internal.LocaleContext"/>
				/// and optional parameters given by the callee (the object owner, in most cases another L20nObject type).
				/// </summary>
				public abstract FTLObject Eval(LocaleContext ctx, params FTLObject[] argv);
			}
		}
	}
}
