// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{
			/// <summary>
			/// The combinator parser used to parse an Entity with all its children.
			/// </summary>
			public static class Entity
			{
				public static void Parse(
					CharStream stream, string identifier,
					Internal.LocaleContext.Builder builder)
				{
					var startingPos = stream.Position;

					try
					{
						// private identifiers start with an underscore
						// and can only be referenced from within an l20n file
						bool isPrivate = (identifier.IndexOf('_') == 0);

						// an optional index is possible
						AST.INode index = null;
						Index.PeekAndParse(stream, out index);

						// White Space is required
						WhiteSpace.Parse(stream, false);

						var valuePos = stream.Position;

						// Now we need the actual value
						var value = Value.Parse(stream);

						if ((value as IO.AST.HashValue) == null && index != null)
						{
							string msg = String.Format(
								"an index was given, but a stringValue was given, while a hashValue was expected",
								stream.ComputeDetailedPosition(valuePos));
							throw new Exceptions.ParseException(msg);
						}

						// an optional attributes collection is possible
						AST.Attributes attributes;
						Attributes.PeekAndParse(stream, out attributes);

						// White Space is optional
						WhiteSpace.Parse(stream, true);

						stream.SkipCharacter('>');
						
						var entityAST = new AST.Entity(identifier, isPrivate, index, value, attributes);
						try
						{
							var entity = (Objects.Entity)entityAST.Eval();
							builder.AddEntity(identifier, entity);
						} catch (Exception e)
						{
							throw new Exceptions.EvaluateException(
								String.Format("couldn't evaluate `{0}`", entityAST.Display()),
								e);
						}
					} catch (Exception e)
					{
						string msg = String.Format(
							"something went wrong parsing an <entity> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new Exceptions.ParseException(msg, e);
					}
				}
			}
		}
	}
}
