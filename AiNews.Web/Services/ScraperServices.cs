using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace AiNews.Web.Services
{

    public class ScraperServices
    {
        public string ScrapeNewsArticle(string newsUrl)
        {
            var html = GetNewsArticleHtml(newsUrl);
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);


            foreach (var node in document.QuerySelectorAll("style, script, nav, header, footer, aside"))
            {
                node.Remove();
            }

            var selectors = new[] { ".post-content", ".entry-content", ".article-content", ".main-content", "article" };

            foreach (var selector in selectors)
            {
                var element = document.QuerySelector(selector);
                if (element != null)
                {
                    var text = element.TextContent.Trim();
                    if (!string.IsNullOrWhiteSpace(text) && text.Length > 200)
                    {
                        return text;
                    }
                }
            }

            var candidates = document.Body.QuerySelectorAll("div, section, p")
                .Select(e => new { Element = e, Text = e.TextContent?.Trim() ?? "" })
                .Where(e => e.Text.Length > 100)
                .OrderByDescending(e => e.Text.Length);

            return candidates.FirstOrDefault()?.Text ?? string.Empty;
        }


        private string GetNewsArticleHtml(string newsUrl)
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                UseCookies = true

            };
            using var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.0.1 Safari/605.1.15");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
            string html = client.GetStringAsync($"{newsUrl}").Result;
            return html;
        }
    }
}
