// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace AST
			{
				/// <summary>
				/// The AST representation for a Builtin.
				/// More Information: <see cref="L20nCore.L20n.FTL.Parsers.Builtin"/>
				/// </summary>
				public sealed class Builtin : INode
				{
					public Builtin(string value)
					{
						m_Value = value;
					}
					
					public L20n.Objects.FTLObject Eval()
					{
						throw new NotImplementedException();
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
}
