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
				/// The AST representation for a member-expression.
				/// More Information: <see cref="L20nCore.L20n.FTL.Parsers.MemberExpression"/>
				/// </summary>
				public sealed class MemberExpression : INode
				{
					public MemberExpression(StringPrimitive identifier, StringPrimitive keyword)
					{
						m_Identifier = identifier;
						m_Keyword = keyword;
					}
					
					public L20n.Objects.FTLObject Eval()
					{
						throw new NotImplementedException();
					}
					
					public string Display()
					{
						return String.Format("{0}[{1}]",
						                     m_Identifier.Display(),
						                     m_Keyword.Display());
					}
					
					private readonly StringPrimitive m_Identifier;
					private readonly StringPrimitive m_Keyword;
				}
			}
		}
	}
}
