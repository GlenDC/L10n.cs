/**
 * This source file is part of the Commercial L20n Unity Plugin.
 *
 * Copyright (c) 2016 Glen De Cauwsemaecker (contact@glendc.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;

namespace L20nCore
{
	namespace IO
	{
		namespace AST
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
