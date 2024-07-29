using System.Text.Json.Serialization;

namespace FrankfurterTest.Entities
{
    public class Currency
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = null!;
        public string Name { get; set; } = null!;

        [JsonIgnore]
        public List<ExchangeRate> BaseExchangeRates { get; set; } = null!;
        [JsonIgnore]
        public List<ExchangeRate> TargetExchangeRates { get; set; } = null!;
    }
}
