// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using L20nCore.Utils;
using L20nCore.Internal;
using L20nCore.Exceptions;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// The <see cref="L20nCore.Objects.BooleanValue"/> class represents a constant boolean value.
		/// </summary>
		public sealed class BooleanValue : L20nObject
		{
			/// <summary>
			/// A public interface to the actual boolean value that
			/// makes up this <see cref="L20nCore.Objects.BooleanValue"/> object.
			/// </summary>
			public bool Value
			{
				get { return m_Value; }
				set { m_Value = value; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.BooleanValue"/> class
			/// with an undefined boolean value.
			/// </summary>
			public BooleanValue()
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.BooleanValue"/> class
			/// wrapping up the given boolean value.
			/// </summary>
			public BooleanValue(bool value)
			{
				m_Value = value;
			}

			/// <summary>
			/// <see cref="L20nCore.Objects.BooleanValue"/> can't be optimized
			/// and will instead return this instance.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// Returns simply this instance, and no actual evaluation takes place.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				return this;
			}
			
			private bool m_Value;
		}
	}
}
