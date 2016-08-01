// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace L20nCore
{
	namespace External
	{
		/// <summary>
		/// A simple interface that allows you to use objects
		/// as external variables using <see cref="L20nCore.External.InfoCollector"/>
		/// to collect the content of your object.
		/// </summary>
		/// <remarks>
		/// In case the stuff you write is flexible,
		/// make sure that you provide your own checks to not allow translators to use
		/// stuff that you don't provide, as that will result in failed translations.
		/// </remarks>
		public interface IHashValue
		{
			void Collect(InfoCollector info);
		}
	}
}
