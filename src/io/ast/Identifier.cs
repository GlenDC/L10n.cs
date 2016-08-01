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
			/// The AST representation for an Identifier.
			/// More Information: <see cref="L20nCore.IO.Parsers.Identifier"/>
			/// </summary>
			public sealed class Identifier : INode
			{
				public Identifier(string value)
				{
					m_Value = value;
				}
				
				public Objects.L20nObject Eval()
				{
					return new Objects.Identifier(m_Value).Optimize();
				}
				
				public string Display()
				{
					return m_Value;
				}

				private readonly string m_Value;
			}
		}
	}
}
