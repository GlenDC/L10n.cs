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
				/// The AST representation for a NUmber.
				/// More Information: <see cref="L20nCore.L20n.FTL.Parsers.Number"/>
				/// </summary>
				public sealed class Number : INode
				{
					public Number(string rawValue)
					{
						try
						{
							m_Value = Convert.ToDouble(rawValue);
						}
						catch(Exception e)
						{
							throw new ParseException("<number> instance could not be created", e);
						}
					}
					
					public L20n.Objects.FTLObject Eval()
					{
						throw new NotImplementedException();
					}
					
					public string Display()
					{
						return m_Value.ToString();
					}
					
					private readonly double m_Value;
				}
			}
		}
	}
}
