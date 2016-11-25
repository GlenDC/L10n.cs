// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace L20n
	{
		namespace FTL
		{
			namespace Parsers
			{	
				/// <summary>
				/// The combinator parser used to parse an Argument.
				/// An argument can be either an expression or a keyword-argument.
				/// </summary>
				public static class Argument
				{
					public static L20n.FTL.AST.INode Parse(CharStream stream)
					{
						L20n.FTL.AST.INode result;

						// if it's an identifier, it could be either simply be an identifier,
						// or it could actually be a keyword-argument
						if (Identifier.PeekAndParse(stream, out result))
						{
							// ignore any whitespace
							WhiteSpace.Parse(stream);

							// if we now encounter a `=` char, we'll assume it's a keyword-argument,
							// and finish the parsing of that element,
							// otherwise we'll assume it's simply an identifier and return early
							if(stream.PeekNext() != '=')
								return result;

							L20n.FTL.AST.INode value = QuotedPattern.Parse(stream);
							return new L20n.FTL.AST.KeywordArgument(
								result as L20n.FTL.AST.StringPrimitive,
								value);
						}

						// it's not an identifier, so is must be any non-identifier expression
						return Expresson.Parse(stream);
					}
				}
			}
		}
	}
}
