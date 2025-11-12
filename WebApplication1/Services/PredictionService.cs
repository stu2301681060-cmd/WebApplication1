using System.Net.Http.Json;

namespace WebApplication1.Services
{
    public class PredictionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _flaskPredictUrl;

        public PredictionService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _flaskPredictUrl = configuration["FlaskApi:PredictUrl"] ?? "https://proekt-cwk0.onrender.com/predict";
        }

        public async Task<Dictionary<string, decimal>> PredictAsync(Dictionary<string, decimal> history, int daysAhead = 5)
        {
            var payload = new
            {
                history,
                days_ahead = daysAhead
            };

            var response = await _httpClient.PostAsJsonAsync(_flaskPredictUrl, payload);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Flask API returned: {response.StatusCode}");

            var predictions = await response.Content.ReadFromJsonAsync<Dictionary<string, decimal>>();
            return predictions ?? new Dictionary<string, decimal>();
        }
    }
}
