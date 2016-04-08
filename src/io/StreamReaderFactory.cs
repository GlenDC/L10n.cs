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
using System.IO;
using System.Collections.Generic;

using L20nCore.Utils;

namespace L20nCore
{
	namespace IO
	{
		/// <summary>
		/// A static class with the sole goal of creating <c>StreamReader</c> instances.
		/// The reason why this is abstracted away in this class, is so that you can override the actual
		/// creation method in case you require a custom method
		/// that goes beyond the default C# StreamReader constructor.
		/// </summary>
		public static class StreamReaderFactory
		{
			/// <summary>
			/// If not null, it will be used as the new creation method to make StreamReader instances.
			/// Will reset to the default creation logic in case the given <c>callback</c> is <c>null</c>.
			/// </summary>
			public static void SetCallback(DelegateType callback)
			{
				m_Delegate.Set(callback);
			}

			/// <summary>
			/// Return a new StreamReader instance for the <c>UTF8</c> resource at the given <c>path</c>.
			/// </summary>
			public static StreamReader Create(string path)
			{
				return m_Delegate.MapOrElse(
					(cb) => cb(path, System.Text.Encoding.UTF8, false),
					() => new StreamReader(path, System.Text.Encoding.UTF8, false));
			}

			public delegate StreamReader DelegateType
				(string path,System.Text.Encoding encoding,bool detectEncoding);
			
			private static Option<DelegateType> m_Delegate =
				new Option<DelegateType>();
		}
	}
}