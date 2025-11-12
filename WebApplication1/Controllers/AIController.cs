using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace WebApplication1.Controllers
{
    [Route("AI")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public AIController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpPost("/analyze")]
        public async Task<IActionResult> Analyze([FromBody] object data)
        {
            using var client = new HttpClient();
            var response = await client.PostAsync("https://proekt-cwk0.onrender.com/analyze",
                new StringContent(data.ToString(), System.Text.Encoding.UTF8, "application/json"));

            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }


    }


    public class AIRequestModel
    {
        [JsonProperty("history")]
        public Dictionary<string, decimal> History { get; set; }

        [JsonProperty("days_ahead")]
        public int DaysAhead { get; set; }
    }
}
