// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using L20nCore.Utils;
using L20nCore.Internal;
using L20nCore.Exceptions;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// The <see cref="L20nCore.Objects.Attributes"/> represents a collection of atttributes
		/// containing <see cref="L20nCore.Objects.L20nObject"/> values matched with an identifier.
		/// </summary>
		public sealed class Attributes : L20nObject
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Attributes"/> class.
			/// </summary>
			public Attributes(Dictionary<string, L20nObject> items)
			{
				m_Items = new Dictionary<string, L20nObject>(items);
				
				if (m_Items.Count == 0)
				{
					Logger.Warning(
						"creating an attributes collection with no children is useless and a mistake, " +
						"this object will make any translation that makes use of it fail");
				}
			}
			
			/// <summary>
			/// Can't be optimized and will be always returning itself.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}
			
			/// <summary>
			/// Returns the value that matches the hash key given as the first given parameter.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				if (m_Items.Count == 0 || argv.Length < 1)
				{
					return null;
				}
				
				var first = argv [0].Eval(ctx);
				
				Identifier id = first as Identifier;
				if (id == null)
				{
					var str = first as StringOutput;
					if (str == null)
					{
						Logger.Warning("Attributes: first variadic argument couldn't be evaluated as an <Identifier>");
						return id;
					}
					
					id = new Identifier(str.Value);
				}	
				
				L20nObject obj;
				if (!m_Items.TryGetValue(id.Value, out obj))
				{
					Logger.WarningFormat("{0} is not a valid <identifier>", id.Value);
					return null;
				}

				return obj;
			}
			
			private readonly Dictionary<string, L20nObject> m_Items;
		}
	}
}
