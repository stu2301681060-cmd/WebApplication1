using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class CurrencyHistoryCache
    {
        public int Id { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Column(TypeName = "text")]  // or "jsonb"
        public string DataJson { get; set; }
    }
}
