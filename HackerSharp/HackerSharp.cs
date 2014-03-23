using System;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

namespace HackerSharpAPI
{
	public class HackerSharp
	{
		private const string baseUrl = "https://news.ycombinator.com/";
		private const string newsUrl = "news";
		private const string newestUrl = "newest";
		private const string askUrl = "ask";
		private const string jobsUrl = "jobs";

		private const string usersUrl = "user?id={0}";
		private const string itemUrl = "item?id={0}";

		//private string nextUrl;

		/// <summary>
		/// News this instance.
		/// </summary>
		public async Task<List<HackerItem>> News(bool nextPage = false){
			List<HackerItem> items = new List<HackerItem> ();

			var httpClient = new HttpClient () { BaseAddress = new Uri (baseUrl) };
			var stream = await httpClient.GetStreamAsync(newsUrl);

			HtmlDocument doc = new HtmlDocument();
			doc.Load(stream);

			return items;
		}
	}
}
