// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace AST
			{
				/// <summary>
				/// The AST representation for a property expression.
				/// More Information: <see cref="L20nCore.L10n.IO.Parsers.Expressions.Property"/>.
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
						m_Identifiers.Add(new Identifier(id));
					}

					public void Add(INode expression)
					{
						m_Identifiers.Add(expression);
					}

					public L10n.Objects.L10nObject Eval()
					{
						var identifiers = new L10n.Objects.L10nObject[m_Identifiers.Count];
						for (int i = 0; i < identifiers.Length; ++i)
							identifiers [i] = m_Identifiers [i].Eval();

						return new L10n.Objects.PropertyExpression(identifiers).Optimize();
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
}
