using System;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Linq;
using System.Diagnostics;

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

        protected HttpClient httpClient;

		private string nextUrl;

        public HackerSharp()
        {
            httpClient = new HttpClient() { BaseAddress = new Uri(baseUrl) };
        }

		/// <summary>
		/// Return popular news
		/// </summary>
		public async Task<List<HackerItem>> News(bool nextPage = false){
			var stream = await httpClient.GetStreamAsync(newsUrl);

			HtmlDocument doc = new HtmlDocument();
			doc.Load(stream);

            var mainTable = doc.DocumentNode.Descendants("table").ToList()[2];
            var rows = mainTable.Descendants("tr").ToList();

            var items = ParseList(rows);
			return items;
		}

        /// <summary>
        /// Return popular news
        /// </summary>
        public async Task<List<HackerItem>> Newest(bool nextPage = false)
        {
			HtmlDocument doc;
			if (nextPage && nextUrl != null) {
				doc = await Load (nextUrl);
			} else {
				doc = await Load (newestUrl);
			}

            var mainTable = doc.DocumentNode.Descendants("table").ToList()[2];
            var rows = mainTable.Descendants("tr").ToList();

            var items = ParseList(rows);
            return items;
        }

        protected async Task<HtmlDocument> Load(string url)
        {
            var stream = await httpClient.GetStreamAsync(url);
            HtmlDocument doc = new HtmlDocument();
            doc.Load(stream);
            return doc;
        }

        protected List<HackerItem> ParseList(List<HtmlNode> rows)
        {
            List<HackerItem> items = new List<HackerItem>();
            HackerItem newsItem = null;

            for (int i = 0; i < rows.Count; i++)
            {
                try
                {
                    var row = rows[i];
                    var tableData = row.Descendants("td");
                    Debug.WriteLine(i);

                    if (tableData != null && tableData.Count() > 0)
                    {
                        if (tableData.Count(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("title")) > 0)
                        {
                            if (i == rows.Count - 1)
                            {
                                nextUrl = row.Descendants("a").ToList()[0].Attributes["href"].Value;
                                break;
                            }

                            newsItem = new HackerItem();
                            ParsingHelpers.ParseUrlTitle(newsItem, row.Descendants("a").ToList());
                            ParsingHelpers.ParseHost(newsItem, row.Descendants("span").ToList());
                        }
                        else if (tableData.Count(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("subtext")) > 0)
                        {
                            //points & user
                            var links = row.Descendants("a").ToList();

                            ParsingHelpers.ParsePoints(newsItem, row.Descendants("span").ToList());
                            ParsingHelpers.ParseUser(newsItem, links);
                            ParsingHelpers.ParseIDAndCommentCount(newsItem, links);

                            items.Add(newsItem);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new HackerException("Could not parse the news feed. See inner exception for details", e);
                }
            }

            return items;
        }
	}
}
