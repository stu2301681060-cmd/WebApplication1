using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class CurrencyHistoryCache
    {
        public int Id { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DataJson { get; set; } 
    }
}
