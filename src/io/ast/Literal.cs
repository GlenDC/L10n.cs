// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace L20nCore
{
	namespace IO
	{
		namespace AST
		{
			/// <summary>
			/// The AST representation for a literal (integer) value.
			/// More Information: <see cref="L20nCore.IO.Parsers.Literal"/>
			/// </summary>
			public sealed class Literal : INode
			{
				public Literal(string raw)
				{
					m_Value = int.Parse(raw);
				}

				public Objects.L20nObject Eval()
				{
					return new Objects.Literal(m_Value).Optimize();
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
