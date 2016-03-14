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
			Assert.Throws<ImportException>(
				() => Translator.ImportManifest("../../../resources/manifest-without-default.json"));
			Assert.Throws<ImportException>(
				() => Translator.ImportManifest("../../../resources/manifest-with-invalid-default.json"));
			Assert.Throws<ImportException>(
				() => Translator.ImportManifest("../../../resources/manifest-without-locales.json"));
			Assert.Throws<ImportException>(
				() => Translator.ImportManifest("../../../resources/manifest-without-resources.json"));
		}

		[Test()]
		public void IdentifierEvalTests()
		{
			Translator.ImportManifest("../../../resources/eval/identifiers/manifest.json");

			Assert.AreEqual("Hello, World!", Translator.Translate("hello"));

			var time = String.Format("{0}:{1}:{2}",
				System.DateTime.Now.Hour,
			    System.DateTime.Now.Minute,
			    System.DateTime.Now.Second);
			Assert.AreEqual(time, Translator.Translate("time"));
			
			var date = String.Format("{0}/{1}/{2}",
			                         System.DateTime.Now.Day,
			                         System.DateTime.Now.Month,
			                         System.DateTime.Now.Year);
			Assert.AreEqual(date, Translator.Translate("date"));

			string greeting = System.DateTime.Now.Hour < 12 ? "Good Morning"
				: (System.DateTime.Now.Hour < 18 ? "Good Afternoon" : "Good Evening");
			
			Assert.AreEqual(greeting, Translator.Translate("greeting"));
			Assert.AreEqual("unknown", Translator.Translate("unknown"));
			Assert.AreEqual(
				Translator.Translate("greeting.evening"),
				Translator.Translate("greeting.evening.normal"));

			Console.WriteLine(Translator.Translate ("timeDateGreeting"));

			// private entities can only be acces from within an lol file
			Assert.AreEqual("_hidden", Translator.Translate("_hidden"));

			// in this case pssst references _hidden (123), so that does work fine
			Assert.AreEqual("the password is 123", Translator.Translate("pssst"));

			// Switching to portuguese

			Translator.SetLocale("pt-BR");
			
			Assert.AreEqual("Olá, Mundo!", Translator.Translate("hello"));
			
			greeting = System.DateTime.Now.Hour < 12 ? "Bom dia"
				: (System.DateTime.Now.Hour < 18 ? "Boa tarde" : "Boa noite");
			
			Assert.AreEqual(greeting, Translator.Translate("greeting"));
			Assert.AreEqual("Boa noite", Translator.Translate("greeting.evening"));
			Assert.AreEqual("Boa noite", Translator.Translate("greeting.evening.late"));
			Assert.AreEqual(
				Translator.Translate("greeting.evening.normal"),
				Translator.Translate("greeting.evening.late"));
			
			Console.WriteLine(Translator.Translate ("timeDateGreeting"));
		}

		[Test()]
		public void SimpleDatabase()
		{
			Translator.AddGlobal("os", () => "win");
			Translator.AddGlobal("screen", () => "desktop");

			var pc = new PerformanceClock("SimpleDatabase");

			pc.Clock("start import database");
			Translator.ImportManifest("../../../resources/manifest.json");
			pc.Clock("database imported (incl. default)");

			pc.Pause();
			Assert.AreEqual("en-US", Translator.DefaultLocale);
			Assert.AreEqual(3, Translator.Locales.Count);
			pc.Continue();
			
			Translator.Translate("l20n");
			Translator.Translate("hello");
			Translator.Translate("kthxbye");
			Translator.Translate("kthxbye.night");
			pc.Clock("translated some simple ids");

			Translator.SetLocale("fr");
			pc.Clock("fr locale imported");

			Translator.Translate("kthxbye");
			Translator.Translate("l20n");
			pc.Clock("translated some simple defaulted ids");
			
			pc.Stop();
		}
	}
}
