using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly CurrencyService _currencyService;

        public CurrencyController(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        
        private List<string> GetCurrencyList() => new()
        {
            
    "AUD", "BGN", "BRL", "CAD", "CHF", "CNY", "CZK", "DKK", "EUR", "GBP", "HKD", "HRK","HUF", "IDR", "ILS", "INR", "ISK", "JPY", "KRW", "MXN", "MYR", "NOK", "NZD", "PHP", "PLN", "RON","RUB", "SEK", "SGD", "THB", "TRY", "USD", "ZAR"   


        };

       
        public IActionResult Index()
        {
            ViewBag.Currencies = GetCurrencyList();
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Convert(string fromCurrency, string toCurrency, decimal amount)
        {
            try
            {
                var converted = await _currencyService.ConvertCurrency(amount, fromCurrency, toCurrency);

                ViewBag.From = fromCurrency;
                ViewBag.To = toCurrency;
                ViewBag.Amount = amount;
                ViewBag.Converted = converted;

                return View("Result");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("Currency/HistoricalData")]
        public async Task<IActionResult> GetHistoricalData(string fromCurrency, string toCurrency, string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                return BadRequest(new { error = "Моля, изберете начален и краен период." });

            if (!DateTime.TryParse(startDate, out DateTime start) || !DateTime.TryParse(endDate, out DateTime end))
                return BadRequest(new { error = "Невалиден формат на датата. Използвайте YYYY-MM-DD." });

            try
            {
                var rates = await _currencyService.GetHistoricalRates(fromCurrency, toCurrency, start, end);

                if (rates == null || rates.Count == 0)
                    return NotFound(new { error = "⚠️ Няма налични данни за избрания период." });

                return Ok(new
                {
                    mode = "historical",
                    from = fromCurrency,
                    to = toCurrency,
                    startDate = start.ToString("yyyy-MM-dd"),
                    endDate = end.ToString("yyyy-MM-dd"),
                    rates
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }





        [HttpPost("Currency/RunAIAnalysis")]
        public async Task<IActionResult> RunAIAnalysis([FromBody] AIRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.FromCurrency) || string.IsNullOrEmpty(request.ToCurrency))
                return BadRequest(new { error = "Невалидни входни данни за AI анализа." });

            try
            {
                var start = DateTime.UtcNow.AddDays(-7);
                var end = DateTime.UtcNow;

                var history = await _currencyService.GetHistoricalRates(request.FromCurrency, request.ToCurrency, start, end);
                if (history == null || history.Count == 0)
                    return NotFound(new { error = "Няма достатъчно исторически данни за избраните валути." });

                var result = await _currencyService.AnalyzeWithPythonAIAsync(history);
                if (result == null || result.Predictions == null || result.Predictions.Count == 0)
                    return BadRequest(new { error = "Flask AI не можа да генерира прогноза." });

                return Ok(new
                {
                    from = request.FromCurrency,
                    to = request.ToCurrency,
                    predictions = result.Predictions,
                    trend = result.Trend,
                    tips = result.Tips
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        
        [HttpPost("/Currency/Analyze")]
        public async Task<IActionResult> Analyze([FromBody] Dictionary<string, decimal> history)
        {
            try
            {
                var aiResult = await _currencyService.AnalyzeWithPythonAIAsync(history);
                if (aiResult == null)
                    return BadRequest(new { message = "AI не върна резултат." });

                return Ok(new
                {
                    trend = aiResult.Trend,
                    predictions = aiResult.Predictions,
                    tips = aiResult.Tips
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        public class AIRequest
        {
            public string FromCurrency { get; set; } = string.Empty;
            public string ToCurrency { get; set; } = string.Empty;
            public decimal Amount { get; set; }
        }
    }
}
