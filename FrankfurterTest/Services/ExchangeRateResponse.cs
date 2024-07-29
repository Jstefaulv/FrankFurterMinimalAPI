using Microsoft.Extensions.Logging.Abstractions;

namespace FrankfurterTest.Services
{
    public class ExchangeRateResponse
    {
        public Dictionary<string, decimal> Rates { get; set; } = null!;
        public string Base { get; set; } = null!;
        public DateTime Date { get; set; }

        public Dictionary<DateTime, Dictionary<string, decimal>> HistoricalRates { get; set; }
        = null!;
    }
}
