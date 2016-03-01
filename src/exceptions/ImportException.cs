/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 - 2017 Glen De Cauwsemaecker (contact@glendc.com)
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

namespace L20n
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
