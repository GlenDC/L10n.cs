// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Utils;

using L20nCore.L10n.Internal;

namespace L20nCore
{
	namespace L10n
	{
		namespace Objects
		{
			/// <summary>
			/// A primitive L10nObject is any L10nObject type that can be
			/// directly translated to a <see cref="System.String"/>.
			/// </summary>
			public abstract class Primitive : L10nObject
			{
				/// <summary>
				/// Compute a string based on the <see cref="L20nCore.L10n.Objects.Primitive"/> object
				/// and its content.
				/// </summary>
				public abstract string ToString(LocaleContext ctx, params L10nObject[] argv);
			}
		}
	}
}
