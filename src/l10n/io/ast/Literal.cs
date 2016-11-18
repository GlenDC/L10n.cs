// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace AST
			{
				/// <summary>
				/// The AST representation for a literal (integer) value.
				/// More Information: <see cref="L20nCore.L10n.IO.Parsers.Literal"/>
				/// </summary>
				public sealed class Literal : INode
				{
					public Literal(string raw)
					{
						m_Value = int.Parse(raw);
					}

					public L10n.Objects.L10nObject Eval()
					{
						return new L10n.Objects.Literal(m_Value).Optimize();
					}

					public string Display()
					{
						return m_Value.ToString();
					}

					private readonly int m_Value;
				}
			}
		}
	}
}
