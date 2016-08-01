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
		/// Thrown when an exception occurs during evaluation of correctly parsed files,
		/// and thus is thrown when there is something wrong related to semantics.
		/// </summary>
		/// <remarks>
		/// Like always the goal is to fail as early as possible.
		/// </remarks>
		public class EvaluateException : ImportException
		{
			public EvaluateException()
				: base()
			{}
			
			public EvaluateException(string message)
				: base(message)
			{}
			
			public EvaluateException(string format, params object[] args)
				: base(string.Format(format, args))
			{}
			
			public EvaluateException(string message, Exception innerException)
				: base(message, innerException)
			{}
			
			public EvaluateException(string format, Exception innerException, params object[] args)
				: base(string.Format(format, args), innerException)
			{}
			
			protected EvaluateException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{}
		}
	}
}
