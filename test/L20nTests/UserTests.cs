/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 Glen De Cauwsemaecker (contact@glendc.com)
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

using L20nCore;
using L20nCore.Exceptions;

namespace L20nCoreTests
{
	[TestFixture()]
	/// <summary>
	/// High level unit tests to check if we can actually parse
	/// and use a manifest file and its content.
	/// </summary>
	public class UserTests
	{
		// an example of a custom global external variable
		private sealed class ScreenInfo : L20nCore.External.IHashValue
		{
			public void Collect(L20nCore.External.InfoCollector info)
			{
				info.Add("width", () => 1920);
				info.Add("height", () => 1080);
			}
		}

		[Test()]
		public void BadManifests()
		{
			var l20n = new Translator();
			Assert.Throws<ImportException>(
				() => l20n.ImportManifest("../../../resources/manifest-without-default.json"));
			Assert.Throws<ImportException>(
				() => l20n.ImportManifest("../../../resources/manifest-with-invalid-default.json"));
			Assert.Throws<ImportException>(
				() => l20n.ImportManifest("../../../resources/manifest-without-locales.json"));
			Assert.Throws<ImportException>(
				() => l20n.ImportManifest("../../../resources/manifest-without-resources.json"));
		}
		
		// an example of a hash external variable
		private sealed class User : L20nCore.External.IHashValue
		{
			public enum Gender
			{
				Male,
				Female,
				Hidden,
			}

			public int Followers { get; set; }

			public User BestFriend;
			private readonly string m_Name;
			private readonly string m_Gender;

			public User(string name, Gender gender, int followers = 0)
			{
				m_Name = name; 
				Followers = followers;

				switch (gender)
				{
					case Gender.Male:
						m_Gender = "masculine";
						break;

					case Gender.Female:
						m_Gender = "feminine";
						break;

					default:
						m_Gender = "default";
						break;
				}

				BestFriend = null;
			}

			public void Collect(L20nCore.External.InfoCollector info)
			{
				info.Add("name", m_Name);
				info.Add("followers", () => Followers);
				info.Add("gender", m_Gender);
				if (BestFriend != null)
					info.Add("friend", BestFriend);
			}
		}

		[Test()]
		public void SimpleManifest()
		{
			var l20n = new Translator();

			l20n.AddGlobal("screen", new ScreenInfo());
			l20n.AddGlobal("isPortrait", () => false);
			l20n.AddGlobal("user", new User("Anonymous", User.Gender.Hidden, 0));
			l20n.AddGlobal("temperature", () => 25);

			// a custom global for this test
			var john = new User("John", User.Gender.Male, 42);
			var maria = new User("Maria", User.Gender.Female, 0);
			john.BestFriend = maria;

			l20n.ImportManifest("../../../resources/eval/identifiers/manifest.json");

			Assert.AreEqual("Hello, World!", l20n.Translate("hello"));

			var time = String.Format("{0}:{1}:{2}",
				System.DateTime.Now.Hour,
			    System.DateTime.Now.Minute,
			    System.DateTime.Now.Second);
			Assert.AreEqual(time, l20n.Translate("time"));
			
			var date = String.Format("{0}/{1}/{2}",
			                         System.DateTime.Now.Day,
			                         System.DateTime.Now.Month,
			                         System.DateTime.Now.Year);
			Assert.AreEqual(date, l20n.Translate("date"));

			string greeting = System.DateTime.Now.Hour < 12 ? "Good Morning"
				: (System.DateTime.Now.Hour < 18 ? "Good Afternoon" : "Good Evening");
			
			Assert.AreEqual(greeting, l20n.Translate("greeting"));
			Assert.AreEqual("unknown", l20n.Translate("unknown"));
			Assert.AreEqual(
				l20n.Translate("greeting.evening"),
				l20n.Translate("greeting.evening.normal"));

			Assert.AreEqual(
				"Data Connectivity Settings",
				l20n.Translate("dataSettings"));
			
			Assert.AreEqual(
				"Landscape mode active!",
				l20n.Translate("orientationActive"));
			
			Assert.AreEqual(
				"She's a Fox and is 12 years old.",
				l20n.Translate("about_fox"));
			Assert.AreEqual(
				"He's a Dog and is 9 years old.",
				l20n.Translate("about_dog"));
			Assert.AreEqual(
				"She's a Wolf and is 42 years old.",
				l20n.Translate("about_wolf"));
			Assert.AreEqual(
				"He's a Bear and is 20 years old.",
				l20n.Translate("about_bear"));
			Assert.AreEqual(
				"She's a Lion and is 19 years old.",
				l20n.Translate("about_lion"));
			Assert.AreEqual(
				l20n.Translate("about_lion_alt"),
				l20n.Translate("about_lion"));
			
			Assert.AreEqual(
				"It said: \"The weather is awesome!\"",
				l20n.Translate("user_talked_about_temperature"));

			Console.WriteLine(l20n.Translate("timeDateGreeting"));

			// private entities can only be acces from within an lol file
			Assert.AreEqual("_hidden", l20n.Translate("_hidden"));

			// in this case pssst references _hidden (123), so that does work fine
			Assert.AreEqual("the password is 123", l20n.Translate("pssst"));

			// translations using an external variable
			Assert.AreEqual(
				"John shared your post.",
				l20n.Translate("shared_compact", "user", john));
			Assert.AreEqual(
				"There are 42 apples.",
				l20n.Translate("apples_sentence", "amount", 42));
			Console.WriteLine(l20n.Translate("apples_sentence", "amount", 42));
			Assert.AreEqual(
				"There is one apple.",
				l20n.Translate("apples_sentence", "amount", 1));
			Assert.AreEqual(
				"There are no apples.",
				l20n.Translate("apples_sentence", "amount", 0));
			Assert.AreEqual(
				"Attachment too big:\n\t45 MB.",
				l20n.Translate("tooBig", "sizeInKB", 46080));
			Assert.AreEqual(
				"John shared your post to his 42 followers.",
				l20n.Translate("shared", "user", john));
			Console.WriteLine(l20n.Translate("shared", "user", john));
			Console.WriteLine(l20n.Translate("shared", "user", maria));
			if (john.BestFriend != null)
				Console.WriteLine(l20n.Translate("best_friend", "user", john));
			Console.WriteLine(l20n.Translate("personal_greeting", "user", "Bianca"));
			Console.WriteLine(l20n.Translate("personal_lucky_greeting",
				new string[] {"user", "lucky_number"},
				new L20nCore.Objects.L20nObject[] {
					new L20nCore.Objects.StringOutput("Bianca"),
				new L20nCore.Objects.Literal(new Random().Next())}));

			// Switching to portuguese

			l20n.SetLocale("pt-BR");
			
			Assert.AreEqual("Olá, Mundo!", l20n.Translate("hello"));
			
			greeting = System.DateTime.Now.Hour < 12 ? "Bom dia"
				: (System.DateTime.Now.Hour < 18 ? "Boa tarde" : "Boa noite");
			
			Assert.AreEqual(greeting, l20n.Translate("greeting"));
			Assert.AreEqual("Boa noite", l20n.Translate("greeting.evening"));
			Assert.AreEqual("Boa noite", l20n.Translate("greeting.evening.late"));
			Assert.AreEqual(
				l20n.Translate("greeting.evening.normal"),
				l20n.Translate("greeting.evening.late"));
			
			Console.WriteLine(l20n.Translate("timeDateGreeting"));

			john.Followers = 31;
			Console.WriteLine(l20n.Translate("shared", "user", john));
			Console.WriteLine(l20n.Translate("shared", "user", maria));
			if (john.BestFriend != null)
				Console.WriteLine(l20n.Translate("best_friend", "user", john));
			Console.WriteLine(l20n.Translate("personal_greeting", "user", "Bianca"));
			Console.WriteLine(l20n.Translate("personal_lucky_greeting",
				new string[] {"user", "lucky_number"},
				new L20nCore.Objects.L20nObject[] {
					new L20nCore.Objects.StringOutput("Bianca"),
					new L20nCore.Objects.Literal(new Random().Next())
			}));
			Console.WriteLine(l20n.Translate("tooBig", "sizeInKB", 46080));
			
			// Switching to slovenian
			
			l20n.SetLocale("sl");

			Console.WriteLine(l20n.Translate("remaining"));
		}

		[Test()]
		public void MozillasManifest()
		{
			var l20n = new Translator();

			l20n.AddGlobal("os", () => "win");

			var pc = new PerformanceClock("SimpleDatabase");

			pc.Clock("start import database");
			l20n.ImportManifest("../../../resources/manifest.json");
			pc.Clock("database imported (incl. default)");
			
			l20n.Translate("l20n");
			l20n.Translate("hello");
			l20n.Translate("kthxbye");
			l20n.Translate("kthxbye.night");
			pc.Clock("translated some simple ids");

			l20n.SetLocale("fr");
			pc.Clock("fr locale imported");
			
			l20n.Translate("l20n");
			l20n.Translate("hello");
			l20n.Translate("kthxbye");
			l20n.Translate("kthxbye.night");
			pc.Clock("translated some simple ids");
			
			pc.Stop();
		}
	}
}
