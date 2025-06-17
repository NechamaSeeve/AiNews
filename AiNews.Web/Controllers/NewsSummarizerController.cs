using AiNews.Web.Model;
using AiNews.Web.Services;
using AngleSharp.Io;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AiNews.Web.Controllers
{
    public class OllamaResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; set; }
    }
    public class Summary
    {
        public string NewsSummary { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class NewsSummarizerController : ControllerBase
    {
        [HttpPost("Summarize")]
        public Summary Summarize(NewsViewModel vm)
        {
            var scraperService = new ScraperServices();
            string html = scraperService.GetNewsArticalHtml(vm.NewsUrl);
            string text = scraperService.ExtractCleanText(html);
            var prompt = $"as a News reporter give a full summary of the news based on this text {text}.";


            var ollamaRequest = new
            {
                model = "llama3",
                prompt = prompt,
                stream = false
            };
            var json = JsonSerializer.Serialize(ollamaRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var response = client.PostAsync("https://api.lit-ai-demo.com/api/generate", content).Result;
            if (!response.IsSuccessStatusCode)
            {
                return new Summary { NewsSummary = "An error occurred while summarizing." };
            }


            var result = response.Content.ReadFromJsonAsync<OllamaResponse>().Result;
            return new Summary { NewsSummary = result.Response };

          
        }


    }
 }

