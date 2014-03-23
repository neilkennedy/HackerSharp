using NUnit.Framework;
using System;
using HackerSharpAPI;
using System.Collections.Generic;

namespace HackerTest
{
	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public async void TestCase ()
		{
			HackerSharp hackerNews = new HackerSharp ();
			List<HackerItem> items = await hackerNews.News ();
		}
	}
}
