// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using L20nCore.Common;
using L20nCore.Common.Utils;
using L20nCore.Common.Exceptions;

using L20nCore.L20n.Objects;

namespace L20nCore
{
	namespace L20n
	{
		namespace Internal
		{
			/// <summary>
			/// The LocaleContext is used by the L20n Objects to
			/// get access to Builtins, Messages and where they can push, use, and pop external variables.
			/// </summary>
			public sealed class LocaleContext
			{	
				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L20n.Internal.LocaleContext"/> class.
				/// </summary>
				public LocaleContext(LocaleContext parent = null)
				{
					m_Parent = parent;
				}

				private LocaleContext m_Parent;
			}
		}
	}
}
