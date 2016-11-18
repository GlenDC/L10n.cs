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
			/// Thrown when an exception occurs during the parsing of Localizeble-Objects-List files,
			/// and thus is thrown when there is something wrong related to syntax.
			/// </summary>
			/// <remarks>
			/// Like always the goal is to fail as early as possible.
			/// </remarks>
			public class ParseException : ImportException
			{
				public ParseException()
					: base()
				{}
				
				public ParseException(string message)
					: base(message)
				{}
				
				public ParseException(string format, params object[] args)
					: base(string.Format(format, args))
				{}
				
				public ParseException(string message, Exception innerException)
					: base(message, innerException)
				{}
				
				public ParseException(string format, Exception innerException, params object[] args)
					: base(string.Format(format, args), innerException)
				{}
				
				protected ParseException(SerializationInfo info, StreamingContext context)
					: base(info, context)
				{}
			}
		}
	}
}
