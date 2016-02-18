using System;
using NUnit.Framework;

using L20n;

namespace L20nTests
{
	[TestFixture()]
	public class MainTest
	{
		[Test()]
		public void LoadLocaleFiles()
		{
			Database database = new Database();
			database.Import("../../../resources/manifest.json");

			Assert.AreEqual("en-US", database.DefaultLocale);
			Assert.AreEqual(3, database.Locales.Count);
		}
	}
}

