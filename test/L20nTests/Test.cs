using System;
using System.IO;
using NUnit.Framework;

using L20n;

namespace L20nTests
{
	[TestFixture()]
	public class MainTest
	{
		[Test()]
		public void BadManifests()
		{
			Database database = new Database();
			
			Assert.Throws<IOException>(
				() => database.Import("../../../resources/manifest-without-default.json"));
			Assert.Throws<IOException>(
				() => database.Import("../../../resources/manifest-with-invalid-default.json"));
			Assert.Throws<IOException>(
				() => database.Import("../../../resources/manifest-without-locales.json"));
			Assert.Throws<IOException>(
				() => database.Import("../../../resources/manifest-without-resources.json"));

			// if you have multiple manifest files (not recommended),
			// the default_locale value becomes optional.
			database.Import("../../../resources/manifest.json");
			database.Import("../../../resources/manifest-without-default.json");
		}

		[Test()]
		public void SimpleDatabase()
		{
			Database database = new Database();
			database.Import("../../../resources/manifest.json");

			Assert.AreEqual("en-US", database.DefaultLocale);
			Assert.AreEqual(3, database.Locales.Count);

			database.LoadLocale();
		}
	}
}

