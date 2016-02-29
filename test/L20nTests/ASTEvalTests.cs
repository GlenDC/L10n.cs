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
using NUnit.Framework;

using L20n.IO;
using L20n.IO.Parsers;
using L20n.IO.Parsers.Expressions;
using System.Collections.Generic;

namespace L20nTests
{
	[TestFixture()]
	public class ASTEvalTests
	{
		[Test()]
		public void ImportStatements()
		{
			var entities = new List<L20n.Types.Entity>();
			Assert.IsTrue(L20n.IO.LocalizbleObjectsList.Parse(
				"../../../resources/eval/import/main.l20n", entities));
			Assert.AreEqual(7, entities.Count);

			// an import statement should be of type StringValue
			// otherwise you'll get an exception at evaluation time
			var invalid = ImportStatement.Parse (NC ("import(42)"))
				as L20n.Types.AST.ImportStatement;
			Assert.Throws<IOException>(
				() => invalid.Evaluate(out entities));

			ImportStatement.Parse(NC("import('some path')"));
		}

		[Test()]
		public void NonExistingPaths()
		{
			var entities = new List<L20n.Types.Entity>();

			// non-existing main file
			Assert.Throws<IOException>(
				() => L20n.IO.LocalizbleObjectsList.Parse(
				"../../../resources/eval/import/nope.l20n", entities));

			// non-existing import file
			Assert.Throws<IOException>(
				() => L20n.IO.LocalizbleObjectsList.Parse(
				"../../../resources/eval/import/invalid-import.l20n", entities));
		}
		
		private CharStream NC(string buffer)
		{
			return new CharStream(buffer);
		}
	}
}
