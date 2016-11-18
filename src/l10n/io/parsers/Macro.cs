// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;

using L20nCore.Common.IO;
using L20nCore.Common.Exceptions;

using L20nCore.L10n.Internal;

namespace L20nCore
{
	namespace L10n
	{
		namespace IO
		{
			namespace Parsers
			{
				/// <summary>
				/// The combinator parser used to parse a Macro,
				/// a function defined within a l10n Resource File.
				/// </summary>
				public static class Macro
				{
					public static void Parse(
					CharStream stream,
					string identifier, LocaleContext.Builder builder)
					{
						var startingPos = stream.Position;

						try
						{
							var macroAST = new L10n.IO.AST.Macro(identifier);

							stream.SkipCharacter('(');
							WhiteSpace.Parse(stream, true);

							// variables are optional,
							// but we do have them, we need at least one (duh)
							if (Expressions.Variable.Peek(stream))
							{
								macroAST.AddParameter(Macro.ParseVariable(stream));

								// more than 1 is possible as well
								while (stream.SkipIfPossible(','))
								{
									WhiteSpace.Parse(stream, true);
									macroAST.AddParameter(Macro.ParseVariable(stream));
								}
							}

							stream.SkipCharacter(')');
							WhiteSpace.Parse(stream, false);

							stream.SkipCharacter('{');
							WhiteSpace.Parse(stream, true);

							// Parse the Actual Macro Expression
							macroAST.SetExpression(Expression.Parse(stream));

							WhiteSpace.Parse(stream, true);
							stream.SkipCharacter('}');
							WhiteSpace.Parse(stream, true);
							stream.SkipCharacter('>');

							// return the fully parsed macro
							try
							{
								var macro = (Objects.Macro)macroAST.Eval();
								builder.AddMacro(identifier, macro);
							} catch (Exception e)
							{
								throw new EvaluateException(
								String.Format("couldn't evaluate `{0}`", macroAST.Display()),
								e);
							}
						} catch (Exception e)
						{
							string msg = String.Format(
							"something went wrong parsing a <macro> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
							throw new ParseException(msg, e);
						}
					}

					public static bool PeekAndParse(
					CharStream stream,
					string identifier, LocaleContext.Builder builder)
					{
						if (stream.PeekNext() != '(')
						{
							return false;
						}

						Macro.Parse(stream, identifier, builder);
						return true;
					}

					private static string ParseVariable(CharStream stream)
					{
						var variable = Expressions.Variable.Parse(stream) as L10n.IO.AST.Variable;
						WhiteSpace.Parse(stream, true);
						return variable.Value;
					}
				}
			}
		}
	}
}
