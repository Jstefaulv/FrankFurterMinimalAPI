namespace FrankfurterTest.Entities
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public int BaseCurrencyId { get; set; }

        public Currency BaseCurrency { get; set; } = null!;
        public int TargetCurrencyId { get; set; }
        public Currency TargetCurrency { get; set; } = null!;
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }

    }
}
