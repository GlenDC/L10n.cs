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
			namespace L10n
			{
				/// <summary>
				/// The AST representation for an attribute expression.
				/// More Information: <see cref="L20nCore.IO.Parsers.Expressions.Attribute"/>.
				/// </summary>
				public sealed class AttributeExpression : INode
				{
					public AttributeExpression(INode root, INode identifier, INode propertyExpression)
					{
						m_Root = root;
						m_Identifier = identifier;
						m_PropertyExpression = propertyExpression;
					}

					public Objects.L20nObject Eval()
					{
						var identifier = m_Identifier.Eval();
						var root = m_Root.Eval();
						var propertyExpression =
						m_PropertyExpression == null ? null : m_PropertyExpression.Eval();
						if (root == null || identifier == null)
							return null;

						var attributeExpression = new Objects.AttributeExpression(root, identifier, propertyExpression);
						return attributeExpression.Optimize();
					}

					public string Display()
					{
						var str = String.Format("{0}::[{1}]",
					                     m_Root.Display(),
					                     m_Identifier.Display());

						return String.Format("{0}.{1}", str, m_PropertyExpression.Display());
					}

					private readonly INode m_PropertyExpression;
					private readonly INode m_Identifier;
					private readonly INode m_Root;
				}
			}
		}
	}
}
