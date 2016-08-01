// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// A primitive L20nObject is any L20nObject type that can be
		/// directly translated to a <see cref="System.String"/>. 
		/// </summary>
		public abstract class Primitive : L20nObject
		{
			/// <summary>
			/// Compute a string based on the <see cref="L20nCore.Objects.Primitive"/> object
			/// and its content.
			/// </summary>
			public abstract string ToString(LocaleContext ctx, params L20nObject[] argv);
		}
	}
}
