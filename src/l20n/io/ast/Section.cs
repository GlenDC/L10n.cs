// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L20n
	{
		namespace IO
		{
			namespace AST
			{
				/// <summary>
				/// The AST representation for a Section.
				/// More Information: <see cref="L20nCore.L20n.IO.Parsers.Section"/>
				/// </summary>
				public sealed class Section : INode
				{
					public Section(Keyword keyword)
					{
						if (keyword == null)
							throw new ParseException("a <section> requires a <keyword>");
						m_Keyword = keyword;
					}
					
					public L20n.Objects.L20nObject Eval()
					{
						throw new NotImplementedException();
					}
					
					public string Display()
					{
						return "[[ " + m_Keyword.Display() + " ]]";
					}
					
					private readonly Keyword m_Keyword;
				}
			}
		}
	}
}
