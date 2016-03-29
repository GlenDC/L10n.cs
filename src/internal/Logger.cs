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

namespace L20nCore
{
	namespace Internal
	{
		/// <summary>
		/// A simple static class to Log warnings.
		/// The logic used to actually log the message,
		/// can be overriden to use the used environment rather than the System calls.
		/// </summary>
		public static class Logger
		{
			/// <summary>
			/// The Current Locale used by L20n, used to provide extra context to the logs.
			/// </summary>
			public static string CurrentLocale { get; set; }

			/// <summary>
			/// When <c>cb</c> is <c>null</c>, the default log-logic will be used,
			/// otherwise the default log-logic will be replaced with the given callback.
			/// </summary>
			public static void SetWarningCallback(LogDelegate cb)
			{
				s_CustomWarning = cb;
			}

			/// <summary>
			/// Log a message as a warning.
			/// </summary>
			public static void Warning(string message)
			{
				if (s_CustomWarning != null)
				{
					s_CustomWarning(
						String.Format("[L20n][{0}] {1}",
					    	CurrentLocale, message));
				} else
				{
					Console.WriteLine("[L20n][{0}][WARNING] {1}",
						CurrentLocale, message);
				}
			}

			/// <summary>
			/// Log a message as a formatted warning.
			/// </summary>
			public static void WarningFormat(string format, params object[] argv)
			{
				var message = String.Format(format, argv);
				Warning(message);
			}

			public delegate void LogDelegate(string msg);

			private static LogDelegate s_CustomWarning = null;
		}
	}
}
