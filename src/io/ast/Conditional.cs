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
			/// The AST representation for a Conditional (branch) expression.
			/// More Information: <see cref="L20nCore.IO.Parsers.Expressions.Logic"/>
			/// </summary>
			public sealed class Conditional : INode
			{	
				public Conditional(INode condition, INode if_true, INode if_false)
				{
					m_Condition = condition;
					m_IfTrue = if_true;
					m_IfFalse = if_false;
				}
				
				public Objects.L20nObject Eval()
				{
					var condition = m_Condition.Eval();
					var if_true = m_IfTrue.Eval();
					var if_false = m_IfFalse.Eval();

					return new Objects.IfElseExpression(
						condition, if_true, if_false).Optimize();
				}
				
				public string Display()
				{
					return String.Format("{0} ? {1} : {2}",
						m_Condition.Display(), m_IfTrue.Display(), m_IfFalse.Display());
				}

				private readonly INode m_Condition;
				private readonly INode m_IfTrue;
				private readonly INode m_IfFalse;
			}
		}
	}
}
