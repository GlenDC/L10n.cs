// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace L20nCore
{
	namespace Common
	{
		namespace Exceptions
		{
			[Serializable]
			/// <summary>
			/// Thrown when an object is unexpected for one reason or another.
			/// An example could be that the type of an object is non-compatible with the expected type.
			/// </summary>
			public class UnexpectedObjectException : ImportException
			{
				public UnexpectedObjectException()
					: base()
				{}
				
				public UnexpectedObjectException(string message)
					: base(message)
				{}
				
				public UnexpectedObjectException(string format, params object[] args)
					: base(string.Format(format, args))
				{}
				
				public UnexpectedObjectException(string message, Exception innerException)
					: base(message, innerException)
				{}
				
				public UnexpectedObjectException(string format, Exception innerException, params object[] args)
					: base(string.Format(format, args), innerException)
				{}
				
				protected UnexpectedObjectException(SerializationInfo info, StreamingContext context)
					: base(info, context)
				{}
			}
		}
	}
}
