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

using L20n.Internal;
using L20n.Utils;

namespace L20n
{
	namespace Objects
	{
		/// <summary>
		/// An abstract /base/ class that represents any type of value
		/// that could be outputed when requesting a global (@).
		/// </summary>
		public abstract class GlobalValue : L20nObject {}

		/// <summary>
		/// A global value type that represents a simple string output.
		/// An example of this could be the current platform of the user.
		/// </summary>
		public sealed class GlobalString : GlobalValue
		{
			public delegate string Delegate();
			private Option<Delegate> m_Callback;

			public GlobalString(Delegate callback)
			{
				m_Callback = new Option<Delegate>(callback);
			}
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return m_Callback.MapOrWarning(
					(callback) => new Option<L20nObject>(new StringOutput(callback())),
					"global string couldn't be retrieved as no delegate was set");
			}
		}

		/// <summary>
		/// A global value type that represents a primitive literal (integer).
		/// An example of this could be the current hour.
		/// </summary>
		public sealed class GlobalLiteral : GlobalValue
		{
			public delegate int Delegate();
			private Option<Delegate> m_Callback;
			
			public GlobalLiteral(Delegate callback)
			{
				m_Callback = new Option<Delegate>(callback);
			}
			
			public override Option<L20nObject> Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return m_Callback.MapOrWarning(
					(callback) => new Option<L20nObject>(new Literal(callback())),
					"global literal couldn't be retrieved as no delegate was set");
			}
		}
	}
}
