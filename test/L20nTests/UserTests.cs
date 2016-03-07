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

using L20n;
using L20n.Exceptions;

namespace L20nTests
{
	[TestFixture()]
	public class UserTests
	{
		[Test()]
		public void BadManifests()
		{
			Database database = new Database();
			
			Assert.Throws<ImportException>(
				() => database.Import("../../../resources/manifest-without-default.json"));
			Assert.Throws<ImportException>(
				() => database.Import("../../../resources/manifest-with-invalid-default.json"));
			Assert.Throws<ImportException>(
				() => database.Import("../../../resources/manifest-without-locales.json"));
			Assert.Throws<ImportException>(
				() => database.Import("../../../resources/manifest-without-resources.json"));
		}
		
		[Test()]
		public void IdentifierEvalTests()
		{
			Database database = new Database ();
			database.Import("../../../resources/eval/identifiers/manifest.json");

			Assert.AreEqual("Hello, World!", database.Translate("hello"));

			var time = String.Format("{0}:{1}:{2}",
				System.DateTime.Now.Hour,
			    System.DateTime.Now.Minute,
			    System.DateTime.Now.Second);
			Assert.AreEqual(time, database.Translate("time"));
			
			var date = String.Format("{0}/{1}/{2}",
			                         System.DateTime.Now.Day,
			                         System.DateTime.Now.Month,
			                         System.DateTime.Now.Year);
			Assert.AreEqual(date, database.Translate("date"));
		}

		[Test()]
		public void SimpleDatabase()
		{
			var pc = new PerformanceClock("SimpleDatabase");

			Database database = new Database();

			pc.Clock("start import database");
			database.Import("../../../resources/manifest.json");
			pc.Clock("database imported (incl. default)");

			pc.Pause();
			Assert.AreEqual("en-US", database.DefaultLocale);
			Assert.AreEqual(3, database.Locales.Count);
			pc.Continue();

			database.LoadLocale("fr");
			pc.Clock("fr locale imported");
			
			pc.Stop();
		}
	}
}

