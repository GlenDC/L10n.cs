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
			/// Thrown when an exception is not found,
			/// while one was expected.
			/// </summary>
			public class ObjectNotFoundException : Exception
			{
				public ObjectNotFoundException()
					: base()
				{}
				
				public ObjectNotFoundException(string message)
					: base(message)
				{}
				
				public ObjectNotFoundException(string format, params object[] args)
					: base(string.Format(format, args))
				{}
				
				public ObjectNotFoundException(string message, Exception innerException)
					: base(message, innerException)
				{}
				
				public ObjectNotFoundException(string format, Exception innerException, params object[] args)
					: base(string.Format(format, args), innerException)
				{}
				
				protected ObjectNotFoundException(SerializationInfo info, StreamingContext context)
					: base(info, context)
				{}
			}
		}
	}
}
