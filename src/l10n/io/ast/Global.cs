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
				/// The AST representation for a Global value.
				/// More Information: <see cref="L20nCore.L10n.IO.Parsers.Global"/>
				/// </summary>
				public sealed class Global : INode
				{	
					public Global(string value)
					{
						m_Value = value;
					}
				
					public L10n.Objects.L10nObject Eval()
					{
						return new L10n.Objects.Global(m_Value).Optimize();
					}
				
					public string Display()
					{
						return String.Format("@{0}", m_Value);
					}
				
					private readonly string m_Value;
				}
			}
		}
	}
}
