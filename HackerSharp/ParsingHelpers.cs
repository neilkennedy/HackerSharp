using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HackerSharpAPI
{
    static class ParsingHelpers
    {
        public static void ParseUrlTitle(HackerItem item, List<HtmlNode> links)
        {
            HtmlNode link;

            if (links.Count == 2)
            {
                link = links[1];
            }
            else
            {
                link = links[0];//if there is no upvote link
            }

            item.URL = link.Attributes["href"].Value;
            item.Title = link.InnerText;
        }

        public static void ParseHost(HackerItem item, List<HtmlNode> spans)
        {
            var hostSpan = spans.Where(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("comhead")).ToList();
            if (hostSpan.Count == 1){
                var host = hostSpan[0].InnerText.Trim();
                item.Host = host.Replace("(", "").Replace(")", "");
            }
        }

        public static void ParsePoints(HackerItem item, List<HtmlNode> spans)
        {
            var pointSpan = spans.Where(x => x.Attributes["id"] != null && x.Attributes["id"].Value.Contains("score")).ToList();
            if (pointSpan.Count == 1)
            {
                item.Points = Regex.Replace(pointSpan[0].InnerText, " p(.*)", "");
            }
        }

        public static void ParseUser(HackerItem item, List<HtmlNode> links)
        {
            var userLink = links.Where(x => x.Attributes["href"].Value.Contains("user")).ToList();

            if (userLink.Count >= 1){
                item.User = userLink[0].InnerText;
            }
        }

        public static void ParseIDAndCommentCount(HackerItem item, List<HtmlNode> links)
        {
            var itemLinks = links.Where(x => x.Attributes["href"].Value.Contains("item")).ToList();

            if (itemLinks.Count >= 1)
            {
                var itemLink = itemLinks[0];
                item.ID = itemLink.Attributes["href"].Value.Replace("item?id=", "");
                item.CommentCount = itemLink.InnerText == "discuss" ? "0" : itemLink.InnerText.Replace(" c(.*)", "");
            }
        }
    }
}
