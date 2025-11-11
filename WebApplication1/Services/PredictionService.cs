using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApplication1.Services
{
    public class PredictionService
    {
        private readonly HttpClient _httpClient;

        public PredictionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, decimal>> PredictAsync(Dictionary<string, decimal> history, int daysAhead = 5)
        {
            var payload = new
            {
                history,
                days_ahead = daysAhead
            };

            var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:5001/predict", payload);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Flask API returned: {response.StatusCode}");
            }

            var predictions = await response.Content.ReadFromJsonAsync<Dictionary<string, decimal>>();
            return predictions ?? new Dictionary<string, decimal>();
        }
    }
}
