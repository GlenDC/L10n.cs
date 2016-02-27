/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 - 2017 Glen De Cauwsemaecker (contact@glendc.com)
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
using System.IO;

namespace L20n
{
	namespace IO
	{
		namespace Parsers
		{	
			public class Value
			{
				public static L20n.Types.Internal.Expressions.Primary Parse(CharStream stream)
				{
					var startingPos = stream.Position;
					
					try {
						L20n.Types.Internal.Expressions.Primary value;
						if(Value.PeekAndParse(stream, out value)) {
							return value;
						}

						throw new IOException("couldn't find valid <value> type");
					}
					catch(Exception e) {
						string msg = String.Format(
							"something went wrong parsing a <value> starting at {0}",
							stream.ComputeDetailedPosition(startingPos));
						throw new IOException(msg, e);
					}
				}

				public static bool PeekAndParse(
					CharStream stream, out L20n.Types.Internal.Expressions.Primary value)
				{
					L20n.Types.AST.Value intermediateValue;
					L20n.Types.Internal.Expressions.Value evaluatedValue;

					if (HashValue.PeekAndParse (stream, out intermediateValue)) {
						if(intermediateValue.Evaluate(out evaluatedValue)) {
							value = evaluatedValue;
							return true;
						}
					}

					if (StringValue.PeekAndParse (stream, out intermediateValue)) {
						if(intermediateValue.Evaluate(out evaluatedValue)) {
							value = evaluatedValue;
							return true;
						}
					}

					value = null;
					return false;
				}
			}
		}
	}
}
