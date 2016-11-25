// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace AST
			{
				/// <summary>
				/// The AST representation for a variable.
				/// More Information: <see cref="L20nCore.L20n.FTL.Parsers.Variable"/>
				/// </summary>
				public sealed class Variable : INode
				{
					public Variable(Keyword keyword)
					{
						m_Keyword = keyword;
					}
					
					public L20n.Objects.FTLObject Eval()
					{
						throw new NotImplementedException();
					}
					
					public string Display()
					{
						return "$" + m_Keyword.Display();
					}
					
					private readonly Keyword m_Keyword;
				}
			}
		}
	}
}
