/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 Glen De Cauwsemaecker (contact@glendc.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Runtime.Serialization;

namespace L20nCore
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
