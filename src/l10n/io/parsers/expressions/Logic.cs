// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace Parsers
			{
				namespace Expressions
				{
					/// <summary>
					/// A parser combinator to parse logic expressions, meaning that it will
					/// parse the type of binary logic expression and the 2 objects on which
					/// the expression will be applied, using their relevant parsers.
					/// </summary>
					public static class Logic
					{
						public static L10n.IO.AST.INode Parse(CharStream stream)
						{
							var startingPos = stream.Position;

							try
							{
								var first = Binary.Parse(stream);
								string op;
								if (stream.ReadReg(@"\s*(\|\||\&\&)", out op))
								{
									WhiteSpace.Parse(stream, true);
									var second = Logic.Parse(stream);
									return new L10n.IO.AST.LogicExpression(
									first, second, op.Trim());
								} else
								{
									return first;
								}
							} catch (Exception e)
							{
								string msg = String.Format(
								"something went wrong parsing an <logical_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
								throw new ParseException(msg, e);
							}
						}
					}
				}
			}
		}
	}
}
