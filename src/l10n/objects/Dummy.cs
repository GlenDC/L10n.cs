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
			/// The dummy object is used by the Database to indicate that an L10nObject that
			/// if it is marked as private it should not translate, as it shouldn't be accessed.
			/// </summary>
			/// <remarks>
			/// I'm not a fan of the approach of this solution to the Private/Public problem,
			/// but it does work within the current System.
			///
			/// If you however read this and read through the other parts of this codebase
			/// and came up with a better solution, feel free to mail me at contact@glendc.com.
			/// Or you can fix it yourself and open a PR at GitHub.
			/// </remarks>
			public sealed class Dummy : L10nObject
			{
				public Dummy()
				{
				}

				// this method should never be called as it is... a dummy object
				public override L10nObject Optimize()
				{
					throw new NotImplementedException();
				}

				// this method should never be called as it is... a dummy object
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}
