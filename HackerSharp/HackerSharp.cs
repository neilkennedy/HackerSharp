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
            var doc = await Load(newestUrl);

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
                var row = rows[i];
                var tableData = row.Descendants("td");

                if (tableData != null && tableData.Count() > 0)
                {
                    if (tableData.Count(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("title")) > 0)
                    {
                        if (i == rows.Count - 1)
                        {
                            nextUrl = row.Descendants("a").ToList()[0].Attributes["href"].Value;
                            break;
                        }

                        //title

                        var link = row.Descendants("a").ToList()[1];
                        newsItem = new HackerItem
                        {
                            URL = link.Attributes["href"].Value,
                            Title = link.InnerText,
                            Host = row.Descendants("span").Where(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("comhead")).ToList()[0].InnerText.Trim().Replace("(", "").Replace(")", "")
                        };
                    }
                    else if (tableData.Count(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("subtext")) > 0)
                    {
                        //points & user
                        var links = row.Descendants("a");
                        var itemLink = links.Where(x => x.Attributes["href"].Value.Contains("item")).ToList()[0];

                        newsItem.Points = row.Descendants("span").ToList()[0].InnerText.Replace(" points", "").Replace(" point", "");
                        newsItem.User = links.Where(x => x.Attributes["href"].Value.Contains("user")).ToList()[0].InnerText;
                        newsItem.ID = itemLink.Attributes["href"].Value.Replace("item?id=", "");
                        newsItem.CommentCount = itemLink.InnerText == "discuss" ? "0" : itemLink.InnerText.Replace(" comments", "");

                        items.Add(newsItem);
                    }
                }
            }

            return items;
        }
	}
}
