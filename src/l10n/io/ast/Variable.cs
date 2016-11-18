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
				/// The AST representation for an exernal object, given by the user.
				/// More Information: <see cref="L20nCore.L10n.IO.Parsers.Variable"/> 
				/// </summary>
				public sealed class Variable : INode
				{
					public string Value
					{
						get { return m_Value; }
					}
				
					public Variable(string value)
					{
						m_Value = value;
					}
				
					public L10n.Objects.L10nObject Eval()
					{
						return new L10n.Objects.Variable(m_Value).Optimize();
					}
				
					public string Display()
					{
						return String.Format("${0}", m_Value);
					}
				
					private readonly string m_Value;
				}
			}
		}
	}
}
