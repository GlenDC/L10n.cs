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
				/// The AST representation for a keyword-argument.
				/// More Information: <see cref="L20nCore.L20n.FTL.Parsers.Argument"/>
				/// </summary>
				public sealed class KeywordArgument : INode
				{
					public KeywordArgument(StringPrimitive id, INode value)
					{
						m_Identifier = id;
						m_DefaultValue = value;
					}
					
					public L20n.Objects.FTLObject Eval()
					{
						throw new NotImplementedException();
					}
					
					public string Display()
					{
						return String.Format("{0} = {1}",
						                     m_Identifier.Display(),
						                     m_DefaultValue.Display());
					}

					private readonly StringPrimitive m_Identifier;
					private readonly INode m_DefaultValue;
				}
			}
		}
	}
}
