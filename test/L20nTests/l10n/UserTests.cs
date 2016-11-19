// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;

using L20nCore.Common.Exceptions;

using L20nCore.L10n;

using L20nCoreTests.Common;

namespace L20nCoreTests
{
	namespace L10n
	{
		[TestFixture()]
		/// <summary>
		/// High level unit tests to check if we can actually parse
		/// and use a manifest file and its content.
		/// </summary>
		public class UserTests
		{
			// an example of a custom global external variable
			private sealed class ScreenInfo : L20nCore.L10n.External.IHashValue
			{
				public void Collect(L20nCore.L10n.External.InfoCollector info)
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
					() => l20n.ImportManifest("../../../resources/l10n/manifest-without-default.json"));
				Assert.Throws<ImportException>(
					() => l20n.ImportManifest("../../../resources/l10n/manifest-with-invalid-default.json"));
				Assert.Throws<ImportException>(
					() => l20n.ImportManifest("../../../resources/l10n/manifest-without-locales.json"));
				Assert.Throws<ImportException>(
					() => l20n.ImportManifest("../../../resources/l10n/manifest-without-resources.json"));
			}

			// an example of a hash external variable
			private sealed class User : L20nCore.L10n.External.IHashValue
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

				public void Collect(L20nCore.L10n.External.InfoCollector info)
				{
					info.Add("name", m_Name);
					info.Add("followers", () => Followers);
					info.Add("gender", m_Gender);
					if (BestFriend != null)
						info.Add("friend", BestFriend);
				}
			}

			// A simple Item test class
			private sealed class Item : L20nCore.L10n.External.IHashValue
			{
				public const string Turtle = "item_turtle";
				public const string Plane = "item_plane";

				public Item (string name, int count)
				{
					m_Name = name;
					m_Count = count;
				}

				public void Collect (L20nCore.L10n.External.InfoCollector info)
				{
					info.Add ("name", m_Name);
					info.Add ("count", m_Count);
				}

				private string m_Name;
				private int m_Count;
			}

			[Test()]
			public void SimpleManifest()
			{
				var l10n = new Translator();

				l10n.AddGlobal("screen", new ScreenInfo());
				l10n.AddGlobal("isPortrait", () => false);
				l10n.AddGlobal("user", new User("Anonymous", User.Gender.Hidden, 0));
				l10n.AddGlobal("temperature", () => 25);

				// a custom global for this test
				var john = new User("John", User.Gender.Male, 42);
				var maria = new User("Maria", User.Gender.Female, 0);
				john.BestFriend = maria;

				l10n.ImportManifest("../../../resources/l10n/eval/identifiers/manifest.json");

				Assert.AreEqual("Hello, World!", l10n.Translate("hello"));

				var time = String.Format("{0}:{1}:{2}",
					System.DateTime.Now.Hour,
				    System.DateTime.Now.Minute,
				    System.DateTime.Now.Second);
				Assert.AreEqual(time, l10n.Translate("time"));

				var date = String.Format("{0}/{1}/{2}",
				                         System.DateTime.Now.Day,
				                         System.DateTime.Now.Month,
				                         System.DateTime.Now.Year);
				Assert.AreEqual(date, l10n.Translate("date"));

				string greeting = System.DateTime.Now.Hour < 12 ? "Good Morning"
					: (System.DateTime.Now.Hour < 18 ? "Good Afternoon" : "Good Evening");

				Assert.AreEqual(greeting, l10n.Translate("greeting"));
				Assert.AreEqual("unknown", l10n.Translate("unknown"));
				Assert.AreEqual(
					l10n.Translate("greeting.evening"),
					l10n.Translate("greeting.evening.normal"));

				Assert.AreEqual(
					"Data Connectivity Settings",
					l10n.Translate("dataSettings"));

				Assert.AreEqual(
					"Landscape mode active!",
					l10n.Translate("orientationActive"));

				Assert.AreEqual(
					"She's a Fox and is 12 years old.",
					l10n.Translate("about_fox"));
				Assert.AreEqual(
					"He's a Dog and is 9 years old.",
					l10n.Translate("about_dog"));
				Assert.AreEqual(
					"She's a Wolf and is 42 years old.",
					l10n.Translate("about_wolf"));
				Assert.AreEqual(
					"He's a Bear and is 20 years old.",
					l10n.Translate("about_bear"));
				Assert.AreEqual(
					"She's a Lion and is 19 years old.",
					l10n.Translate("about_lion"));
				Assert.AreEqual(
					l10n.Translate("about_lion_alt"),
					l10n.Translate("about_lion"));

				Assert.AreEqual(
					"It said: \"The weather is awesome!\"",
					l10n.Translate("user_talked_about_temperature"));

				Console.WriteLine(l10n.Translate("timeDateGreeting"));

				// private entities can only be acces from within an lol file
				Assert.AreEqual("_hidden", l10n.Translate("_hidden"));

				// in this case pssst references _hidden (123), so that does work fine
				Assert.AreEqual("the password is 123", l10n.Translate("pssst"));

				// translations using an external variable
				Assert.AreEqual(
					"John shared your post.",
					l10n.Translate("shared_compact", "user", john));
				Assert.AreEqual(
					"There are 42 apples.",
					l10n.Translate("apples_sentence", "amount", 42));
				Console.WriteLine(l10n.Translate("apples_sentence", "amount", 42));
				Assert.AreEqual(
					"There is one apple.",
					l10n.Translate("apples_sentence", "amount", 1));
				Assert.AreEqual(
					"There are no apples.",
					l10n.Translate("apples_sentence", "amount", 0));
				Assert.AreEqual(
					"Attachment too big:\n\t45 MB.",
					l10n.Translate("tooBig", "sizeInKB", 46080));
				Assert.AreEqual(
					"John shared your post to his 42 followers.",
					l10n.Translate("shared", "user", john));
				Console.WriteLine(l10n.Translate("shared", "user", john));
				Console.WriteLine(l10n.Translate("shared", "user", maria));
				if (john.BestFriend != null)
					Console.WriteLine(l10n.Translate("best_friend", "user", john));
				Console.WriteLine(l10n.Translate("personal_greeting", "user", "Bianca"));
				Console.WriteLine(l10n.Translate("personal_lucky_greeting",
					new List<string> {"user", "lucky_number"},
					new List<L20nCore.L10n.Objects.L10nObject> {
						new L20nCore.L10n.Objects.StringOutput("Bianca"),
					new L20nCore.L10n.Objects.Literal(new Random().Next())}));

				// test for one-value tables
				Assert.AreEqual ("Hello, world!", l10n.Translate ("deep_hello"));

				// some tests to see if we can use a string as an identifier, even
				// if the string is coming from an external variable, should work to allow
				// even more dynamic sentences.
				Assert.AreEqual (
					"I like 42 turtles.",
					l10n.Translate ("statement_fp_like_item", "item", new Item (Item.Turtle, 42)));
				Assert.AreEqual (
					"I like 1 turtle.",
					l10n.Translate ("statement_fp_like_item", "item", new Item (Item.Turtle, 1)));
				Assert.AreEqual (
					"I like 2 planes.",
					l10n.Translate ("statement_fp_like_item", "item", new Item (Item.Plane, 2)));

				// Switching to portuguese

				l10n.SetLocale("pt-BR");

				Assert.AreEqual("Olá, Mundo!", l10n.Translate("hello"));

				greeting = System.DateTime.Now.Hour < 12 ? "Bom dia"
					: (System.DateTime.Now.Hour < 18 ? "Boa tarde" : "Boa noite");

				Assert.AreEqual(greeting, l10n.Translate("greeting"));
				Assert.AreEqual("Boa noite", l10n.Translate("greeting.evening"));
				Assert.AreEqual("Boa noite", l10n.Translate("greeting.evening.late"));
				Assert.AreEqual(
					l10n.Translate("greeting.evening.normal"),
					l10n.Translate("greeting.evening.late"));

				Console.WriteLine(l10n.Translate("timeDateGreeting"));

				john.Followers = 31;
				Console.WriteLine(l10n.Translate("shared", "user", john));
				Console.WriteLine(l10n.Translate("shared", "user", maria));
				if (john.BestFriend != null)
					Console.WriteLine(l10n.Translate("best_friend", "user", john));
				Console.WriteLine(l10n.Translate("personal_greeting", "user", "Bianca"));
				Console.WriteLine(l10n.Translate("personal_lucky_greeting",
					new List<string> {"user", "lucky_number"},
					new List<L20nCore.L10n.Objects.L10nObject> {
						new L20nCore.L10n.Objects.StringOutput("Bianca"),
						new L20nCore.L10n.Objects.Literal(new Random().Next())
				}));
				Console.WriteLine(l10n.Translate("tooBig", "sizeInKB", 46080));

				// some tests to see if we can use a string as an identifier, even
				// if the string is coming from an external variable, should work to allow
				// even more dynamic sentences.
				// These are testing strings used as identifiers in attribute expressions.
				Assert.AreEqual (
					"Gosto de 42 tartarugas pequenas.",
					l10n.Translate ("statement_fp_like_item", "item", new Item (Item.Turtle, 42)));
				Assert.AreEqual (
					"Gosto de 1 tartaruga pequena.",
					l10n.Translate ("statement_fp_like_item", "item", new Item (Item.Turtle, 1)));
				Assert.AreEqual (
					"Gosto de 2 aviões grandes.",
					l10n.Translate ("statement_fp_like_item", "item", new Item (Item.Plane, 2)));
				Assert.AreEqual (
					"Gosto de 1 avião grande.",
					l10n.Translate ("statement_fp_like_item_alt",
					new List<string> {"item", "count"},
					new List<L20nCore.L10n.Objects.L10nObject> {
						new L20nCore.L10n.Objects.StringOutput(Item.Plane),
						new L20nCore.L10n.Objects.Literal(1)}));
				Assert.AreEqual (
					l10n.Translate ("statement_fp_like_item", "item", new Item (Item.Turtle, 503)),
					l10n.Translate ("statement_fp_like_item_alt",
					new List<string> {"item", "count"},
					new List<L20nCore.L10n.Objects.L10nObject> {
						new L20nCore.L10n.Objects.StringOutput(Item.Turtle),
						new L20nCore.L10n.Objects.Literal(503)}));

				// Switching to slovenian

				l10n.SetLocale("sl");

				Console.WriteLine(l10n.Translate("remaining"));
			}

			[Test()]
			public void MozillasManifest()
			{
				var l10n = new Translator();

				l10n.AddGlobal("os", () => "win");

				var pc = new PerformanceClock("SimpleDatabase");

				pc.Clock("start import database");
				l10n.ImportManifest("../../../resources/l10n/manifest.json");
				pc.Clock("database imported (incl. default)");

				l10n.Translate("l20n");
				l10n.Translate("hello");
				l10n.Translate("kthxbye");
				l10n.Translate("kthxbye.night");
				pc.Clock("translated some simple ids");

				l10n.SetLocale("fr");
				pc.Clock("fr locale imported");

				l10n.Translate("l20n");
				l10n.Translate("hello");
				l10n.Translate("kthxbye");
				l10n.Translate("kthxbye.night");
				pc.Clock("translated some simple ids");

				pc.Stop();
			}
			
			[Test()]
			public void ImportTests()
			{
				var l10n = new Translator();
				
				var pc = new PerformanceClock("Deep Database");
				
				pc.Clock("start import database");
				l10n.ImportManifest("../../../resources/l10n/eval/import/manifest.json");

				pc.Clock("Translate Sentences");
				// # Level 0
				Assert.AreEqual("world", l10n.Translate("hello"));
				// # Level 1
				Assert.AreEqual("cup", l10n.Translate("thing"));
				Assert.AreEqual("apple", l10n.Translate("fruit"));
				Assert.AreEqual("bread", l10n.Translate("food"));
				// # Level 2
				Assert.AreEqual("lion", l10n.Translate("animal"));
				Assert.AreEqual("waffy", l10n.Translate("dog"));
				
				pc.Stop();
			}
		}
	}
}
