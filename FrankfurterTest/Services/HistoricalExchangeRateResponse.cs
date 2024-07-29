namespace FrankfurterTest.Services
{
    public class HistoricalExchangeRateResponse
    {
        public string Base { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; } = null!;
    }
}
