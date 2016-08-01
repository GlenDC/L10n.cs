// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace L20nCore
{
	namespace IO
	{
		namespace AST
		{
			/// <summary>
			/// The AST representation for a call expression.
			/// More Information: <see cref="L20nCore.IO.Parsers.Expressions.Call"/>
			/// </summary>
			public sealed class CallExpression : INode
			{
				public CallExpression(INode name)
				{
					m_Name = name;
					m_Parameters = new List<INode>();
				}
				
				public void AddParameter(INode parameter)
				{
					m_Parameters.Add(parameter);
				}
				
				public Objects.L20nObject Eval()
				{
					var name = ((Objects.Identifier)m_Name.Eval()).Value;
					var parameters = new Objects.L20nObject[m_Parameters.Count];
					for (int i = 0; i < parameters.Length; ++i)
						parameters [i] = m_Parameters [i].Eval();

					return new Objects.CallExpression(name, parameters).Optimize();
				}
				
				public string Display()
				{
					var parameters = new string[m_Parameters.Count];
					for (int i = 0; i < parameters.Length; ++i)
						parameters [i] = m_Parameters [i].Display();

					return String.Format("{0}({1})",
						m_Name.Display(), String.Join(",", parameters));
				}

				private readonly INode m_Name;
				private readonly List<INode> m_Parameters;
			}
		}
	}
}
