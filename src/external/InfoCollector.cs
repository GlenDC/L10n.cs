// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using L20nCore.Objects;
using L20nCore.Utils;

namespace L20nCore
{
	namespace External
	{
		/// <summary>
		/// Used to collect the contents of an object that implements
		/// <see cref="L20nCore.External.IHashValue"/>.
		/// </summary>
		public sealed class InfoCollector
		{
			/// <summary>
			/// A pool of available InfoCollectors to reduce the amount of objects needed for creation.
			/// </summary>
			public static ObjectPool<InfoCollector> Pool
			{
				get { return s_Pool; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.External.InfoCollector"/> class.
			/// </summary>
			public InfoCollector()
			{
				m_Info = new Dictionary<string, L20nObject>();
			}

			/// <summary>
			/// Add a <see cref="L20nCore.Objects.Literal"/> value with the given <c>name</c>.
			/// </summary>
			public void Add(string name, int value)
			{
				AddObject(name, new Literal(value));
			}

			/// <summary>
			/// Add a <see cref="L20nCore.Objects.StringOutput"/> value with the given <c>name</c>.
			/// </summary>
			public void Add(string name, string value)
			{
				AddObject(name, new StringOutput(value));
			}

			/// <summary>
			/// Add a <see cref="L20nCore.Objects.BooleanValue"/> value with the given <c>name</c>.
			/// </summary>
			public void Add(string name, bool value)
			{
				AddObject(name, new BooleanValue(value));
			}

			/// <summary>
			/// Add a <see cref="L20nCore.Objects.DelegatedLiteral+Delegate"/> value with the given <c>name</c>.
			/// </summary>
			public void Add(string name, Objects.DelegatedLiteral.Delegate callback)
			{
				AddObject(name, new Objects.DelegatedLiteral(callback));
			}

			/// <summary>
			/// Add a <see cref="L20nCore.Objects.DelegatedString+Delegate"/> value with the given <c>name</c>.
			/// </summary>
			public void Add(string name, Objects.DelegatedString.Delegate callback)
			{
				AddObject(name, new Objects.DelegatedString(callback));
			}
			
			/// <summary>
			/// Add a <see cref="L20nCore.Objects.DelegatedBoolean+Delegate"/> value with the given <c>name</c>.
			/// </summary>
			public void Add(string name, Objects.DelegatedBoolean.Delegate callback)
			{
				AddObject(name, new Objects.DelegatedBoolean(callback));
			}

			/// <summary>
			/// Add a <see cref="L20nCore.External.IHashValue"/> value with the given <c>name</c>.
			/// </summary>
			public void Add(string name, IHashValue value)
			{
				// Prepare the collector
				var info = External.InfoCollector.Pool.GetObject();

				// Collect the given value
				value.Collect(info);

				if (m_Info.Count == 0)
				{
					Internal.Logger.Warning(
						"can't add an external variable that has no information exposed," +
						" please add information by calling `Info.Add(...)`");
					return;
				}

				// Add it as a child to the current object
				AddObject(name, info.Collect());

				info.Clear();
				External.InfoCollector.Pool.ReturnObject(ref info);
			}

			/// <summary>
			/// Collects all the added info and returns it as a <see cref="L20nCore.Objects.HashValue"/>.
			/// THIS SHOULD NOT BE CALLED BY THE CLASS THAT IMPLEMENTS <see cref="L20nCore.External.IHashValue"/>!
			/// </summary>
			public HashValue Collect()
			{
				return new HashValue(m_Info, null);
			}

			/// <summary>
			/// Unreferences all the previously added content.
			/// </summary>
			public void Clear()
			{
				m_Info.Clear();
			}

			private void AddObject(string name, L20nObject value)
			{
				if (m_Info.ContainsKey(name))
				{
					Internal.Logger.WarningFormat(
						"information with the name {0} will be overriden", name);
				}

				m_Info.Add(name, value);
			}

			private static ObjectPool<InfoCollector> s_Pool =
				new ObjectPool<InfoCollector>(32);

			private readonly Dictionary<string, L20nObject> m_Info;
		}
	}
}
