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
			/// <see cref="L20nCore.L10n.Objects.StringOutput"/> is a L10nObject type that
			/// represents a string value, either static or as the result of a L10n Expression.
			/// </summary>
			public sealed class StringOutput : Primitive
			{	
				/// <summary>
				/// A public interface to the actual string value that
				/// makes up this <see cref="L20nCore.L10n.Objects.StringOutput"/> object.
				/// </summary>
				public string Value
				{
					get { return m_Value; }
					set { m_Value = value; }
				}

				private string m_Value;

				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.StringOutput"/> class
				/// with an undefined string value.
				/// </summary>
				public StringOutput()
				{
					m_Value = "";
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="L20nCore.L10n.Objects.StringOutput"/> class
				/// with an initial string value given by the callee of this constructor.
				/// </summary>
				public StringOutput(string value)
				{
					m_Value = value;
				}

				/// <summary>
				/// <see cref="L20nCore.L10n.Objects.StringOutput"/> is already the most primitive L10nType of its kind
				/// and can therefore not be further optimized and simply returns itself as a result.
				/// </summary>
				public override L10nObject Optimize()
				{
					return this;
				}

				/// <summary>
				/// The evaluation of a <see cref="L20nCore.L10n.Objects.StringOutput"/> object
				/// is as simple as returning itself.
				/// </summary>
				public override L10nObject Eval(LocaleContext ctx, params L10nObject[] argv)
				{
					return this;
				}

				/// <summary>
				/// Returns the string value that makes up this
				/// <see cref="L20nCore.L10n.Objects.StringOutput"/> object. 
				/// </summary>
				public override string ToString(LocaleContext ctx, params L10nObject[] argv)
				{
					return m_Value;
				}
			}
		}
	}
}
