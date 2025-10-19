namespace FrankfurterTest.DTOs
{
    public class ExchangeRateDTO
    {
        public int Id { get; set; }
        public int BaseCurrencyId { get; set; }
        public int TargetCurrencyId { get; set; }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
    }
}
