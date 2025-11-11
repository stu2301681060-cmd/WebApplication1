using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using SystemTextJson = System.Text.Json.JsonSerializer; 
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class CurrencyService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CurrencyService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
            _httpClient = new HttpClient();
        }

        
        public async Task<Dictionary<string, decimal>> GetHistoricalRates(string from, string to, DateTime start, DateTime end)
        {
            
            var cached = await _context.CurrencyHistoryCache
                .FirstOrDefaultAsync(x => x.FromCurrency == from && x.ToCurrency == to);

            if (cached == null)
            {
                var apiData = await FetchHistoricalRatesFromApi(from, to, start, end);
                var jsonData = SystemTextJson.Serialize(apiData);

                _context.CurrencyHistoryCache.Add(new CurrencyHistoryCache
                {
                    FromCurrency = from,
                    ToCurrency = to,
                    StartDate = start.Date,
                    EndDate = end.Date,
                    DataJson = jsonData
                });

                await _context.SaveChangesAsync();
                return apiData;
            }

            
            var existingData = SystemTextJson.Deserialize<Dictionary<string, decimal>>(cached.DataJson)
                                ?? new Dictionary<string, decimal>();

            
            if (start >= cached.StartDate && end <= cached.EndDate)
            {
                return existingData
                    .Where(d => DateTime.Parse(d.Key) >= start && DateTime.Parse(d.Key) <= end)
                    .ToDictionary(d => d.Key, d => d.Value);
            }

            
            var fetchStart = start < cached.StartDate ? start : cached.EndDate.AddDays(1);
            var fetchEnd = end > cached.EndDate ? end : cached.EndDate;

            
            var newData = await FetchHistoricalRatesFromApi(from, to, fetchStart, fetchEnd);

            
            foreach (var kv in newData)
                existingData[kv.Key] = kv.Value;

           
            cached.StartDate = start < cached.StartDate ? start : cached.StartDate;
            cached.EndDate = end > cached.EndDate ? end : cached.EndDate;
            cached.DataJson = SystemTextJson.Serialize(existingData);

            _context.CurrencyHistoryCache.Update(cached);
            await _context.SaveChangesAsync();

            
            return existingData
                .Where(d => DateTime.Parse(d.Key) >= start && DateTime.Parse(d.Key) <= end)
                .ToDictionary(d => d.Key, d => d.Value);
        }

      
        private async Task<Dictionary<string, decimal>> FetchHistoricalRatesFromApi(string from, string to, DateTime start, DateTime end)
        {
            var url = $"https://api.frankfurter.app/{start:yyyy-MM-dd}..{end:yyyy-MM-dd}?from={from}&to={to}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API error: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonDocument.Parse(json);

            var rates = new Dictionary<string, decimal>();
            var ratesElement = data.RootElement.GetProperty("rates");

            foreach (var day in ratesElement.EnumerateObject())
            {
                var date = day.Name;
                var rate = day.Value.GetProperty(to).GetDecimal();
                rates[date] = rate;
            }

            return rates;
        }

      
        public async Task<decimal> GetExchangeRate(string fromCurrency, string toCurrency)
        {
            var url = $"https://api.frankfurter.app/latest?from={fromCurrency}&to={toCurrency}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("API request failed.");

            var json = await response.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(json);

            var rates = doc.RootElement.GetProperty("rates");
            if (!rates.TryGetProperty(toCurrency, out var rate))
                throw new Exception("Invalid API response.");

            return rate.GetDecimal();
        }

     
        public async Task<decimal> ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
        {
            var rate = await GetExchangeRate(fromCurrency, toCurrency);
            return Math.Round(amount * rate, 2);
        }


        // 🤖 Flask AI connection
        public async Task<AIResult?> AnalyzeWithPythonAIAsync(Dictionary<string, decimal> history)
        {
            try
            {
                string flaskApiUrl = "http://localhost:5001/analyze";
                var payload = new { history };

                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(flaskApiUrl, content);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Flask AI request failed.");

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<AIResult>(responseJson, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ AI Error: {ex.Message}");
                return null;
            }
        }

        
       public class AIResult
        {
            public string Trend { get; set; } = string.Empty;
            public decimal PercentChange { get; set; }
            public List<decimal> Predictions { get; set; } = new();
            public List<string> Tips { get; set; } = new();
        }
    }
}
