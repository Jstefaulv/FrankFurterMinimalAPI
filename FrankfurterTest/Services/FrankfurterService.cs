namespace FrankfurterTest.Services
{
    public class FrankfurterService
    {
        private readonly HttpClient _httpClient;

        public FrankfurterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ExchangeRateResponse?> GetExchangeRatesAsync(
            string baseCurrency, string targetCurrency)
        {
            var response = await _httpClient.GetAsync($"https://api.frankfurter.app/latest?from={baseCurrency}&to={targetCurrency}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ExchangeRateResponse>();
        }
    }

}

