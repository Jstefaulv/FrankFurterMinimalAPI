namespace FrankfurterTest.DTOs
{
    public class CreateExchangeRateDTO
    {
        public int BaseCurrencyId { get; set; }
        public int TargetCurrencyId { get; set; }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
    }
}
