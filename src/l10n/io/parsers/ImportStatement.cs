// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Collections.Generic;

using L20nCore.L10n.Internal;

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
				/// <summary>
				/// The combinator parser used to parse an import statement,
				/// which means that we will parse the L10n resource file that this
				/// import statement is pointing to.
				/// </summary>
				public static class ImportStatement
				{
					public static void Parse(CharStream stream, LocaleContext.Builder builder)
					{
						var startingPos = stream.Position;
					
						try
						{
							stream.SkipString("import(");
							WhiteSpace.Parse(stream, true);
							var path = PureStringValue.Parse(stream);
							WhiteSpace.Parse(stream, true);
							stream.SkipCharacter(')');

							LocalizableObjectsList.ImportAndParse(path, builder);
						} catch (Exception e)
						{
							string msg = String.Format(
							"something went wrong parsing an <import_statement> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
							throw new ParseException(msg, e);
						}
					}
				}
			}
		}
	}
}
