// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Exceptions;
using System.Collections.Generic;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace AST
			{
				/// <summary>
				/// The AST representation for a call-expression.
				/// More Information: <see cref="L20nCore.L20n.FTL.Parsers.CallExpression"/>
				/// </summary>
				public sealed class CallExpression : INode
				{
					public CallExpression(StringPrimitive builtin, List<INode> args)
					{
						m_Builtin = builtin;
						m_Arguments = args;
					}
					
					public L20n.Objects.FTLObject Eval()
					{
						throw new NotImplementedException();
					}
					
					public string Display()
					{
						string output = m_Builtin.Display() + "(";
						for (int i = 0; i < m_Arguments.Count; ++i)
							output += m_Arguments [i].Display() + ", ";
						return output.TrimEnd(new char[]{',', ' '}) + ")";
					}
					
					private readonly StringPrimitive m_Builtin;
					private readonly List<INode> m_Arguments;
				}
			}
		}
	}
}
