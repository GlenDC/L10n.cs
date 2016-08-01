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
			/// The AST representation for a property expression.
			/// More Information: <see cref="L20nCore.IO.Parsers.Expressions.Property"/>.
			/// </summary>
			public sealed class PropertyExpression : INode
			{
				public PropertyExpression(INode root)
				{
					m_Identifiers = new List<INode>();
					m_Identifiers.Add(root);
				}

				public void Add(string id)
				{
					m_Identifiers.Add(new AST.Identifier(id));
				}

				public void Add(INode expression)
				{
					m_Identifiers.Add(expression);
				}

				public Objects.L20nObject Eval()
				{
					var identifiers = new Objects.L20nObject[m_Identifiers.Count];
					for (int i = 0; i < identifiers.Length; ++i)
						identifiers [i] = m_Identifiers [i].Eval();

					return new Objects.PropertyExpression(identifiers).Optimize();
				}

				public string Display()
				{
					var identifiers = new string[m_Identifiers.Count];
					for (int i = 0; i < identifiers.Length; ++i)
						identifiers [i] = String.Format("[{0}]", m_Identifiers [i].Display());

					return String.Join("", identifiers);
				}

				private readonly List<INode> m_Identifiers;
			}
		}
	}
}
