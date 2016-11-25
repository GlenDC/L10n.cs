// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Text.RegularExpressions;

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
				/// The AST representation for any StringPrimitive.
				/// This can be a keyword, Identifier or Builtin.
				/// Keeping this as one type, makes it easy to 'convert' between one another,
				/// which is sometimes needed during context switching while parsing.
				/// </summary>
				public sealed class StringPrimitive : INode
				{
					public enum Kind : byte
					{
						Keyword = 1,
						Identifier = 2,
						Builtin = 4,
					}

					public StringPrimitive(string value, Kind kind)
					{
						m_Value = value;
					}

					public void Transform(Kind kind)
					{
						Regex reg;

						switch (kind)
						{
							case Kind.Keyword:
								reg = L20n.FTL.Parsers.Keyword.Regex;
								break;
							case Kind.Identifier:
								reg = L20n.FTL.Parsers.Identifier.Regex;
								break;
							case Kind.Builtin:
								reg = L20n.FTL.Parsers.Builtin.Regex;
								break;
						}

						if (!reg.IsMatch(m_Value))
						{
							throw new ParseException("could not transform {0}({1}) to a {2}",
							                         m_Value, m_Kind,
							                         kind);
						}

						m_Kind = kind;
					}
					
					public L20n.Objects.FTLObject Eval()
					{
						throw new NotImplementedException();
					}
					
					public string Display()
					{
						return m_Value;
					}
					
					private readonly string m_Value;
					private Kind m_Kind; 
				}
			}
		}
	}
}
