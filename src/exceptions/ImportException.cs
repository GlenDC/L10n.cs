// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace L20nCore
{
	namespace Exceptions
	{
		[Serializable]
		/// <summary>
		/// Thrown when an exception occurs while importing a Localizeble-Objects-List file.
		/// </summary>
		public class ImportException : Exception
		{
			public ImportException()
				: base()
			{}
			
			public ImportException(string message)
				: base(message)
			{}
			
			public ImportException(string format, params object[] args)
				: base(string.Format(format, args))
			{}
			
			public ImportException(string message, Exception innerException)
				: base(message, innerException)
			{}
			
			public ImportException(string format, Exception innerException, params object[] args)
				: base(string.Format(format, args), innerException)
			{}
			
			protected ImportException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{}
		}
	}
}
