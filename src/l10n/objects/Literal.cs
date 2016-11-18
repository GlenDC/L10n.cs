// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Utils;

using L20nCore.L10n.Internal;

namespace L20nCore
{
	namespace L10n
	{
		namespace Objects
		{
			/// <summary>
			/// <see cref="L20nCore.L10n.Objects.Literal"/> is a L10nObject type that
			/// represents a fixed whole number. This can be either a static number as given
			/// in an L10n Language File, or the result of a computation.
			/// </summary>
			public sealed class Literal : Primitive
			{
				/// <summary>
				/// A public interface to the actual integer value that
				/// makes up this <see cref="L20nCore.L10n.Objects.Literal"/> object.
				/// </summary>
				public int Value
				{
					get { return m_Value; }
					set { m_Value = value; }
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.Literal"/> class
				/// with an undefined integer value.
				/// </summary>
				public Literal()
				{
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.Literal"/> class
				/// with a defined initial integer value given by callee of this constructor.
				/// </summary>
				public Literal(int value)
				{
					m_Value = value;
				}

				/// <summary>
				/// <see cref="L20nCore.L10n.Objects.Literal"/> is already the most primitive L10nType of its kind
				/// and can therefore not be further optimized and simply returns itself as a result.
				/// </summary>
				public override L10nObject Optimize()
				{
					return this;
				}

				/// <summary>
				/// The evaluation of a <see cref="L20nCore.L10n.Objects.Literal"/> object
				/// is as simple as returning itself.
				/// </summary>
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
				{
					return this;
				}

				/// <summary>
				/// Returns the string representation of the <see cref="System.Int32"/> value
				/// contained in this <see cref="L20nCore.L10n.Objects.Literal"/> object. 
				/// </summary>
				public override string ToString(LocaleContext ctx, params L10nObject[] argv)
				{
					return m_Value.ToString();
				}
			
				private int m_Value;
			}
		}
	}
}
