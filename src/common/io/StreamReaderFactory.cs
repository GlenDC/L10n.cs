// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Collections.Generic;

using L20nCore.Common.Utils;

namespace L20nCore
{
	namespace Common
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
}