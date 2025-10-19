using FrankfurterTest.DTOs;
using FrankfurterTest.Entities;

namespace FrankfurterTest.Repositories
{
    public interface IRepositoryExchangeRates
    {
        Task<int> CreateExchangeRate(ExchangeRate rate);
        Task<List<ExchangeRate>> GetAll(PaginationDTO paginationDTO);
        Task<ExchangeRate?> GetById(int id);
        Task Update(ExchangeRate exchangeRate);
        Task Delete(int id);

        #region ByBasecurrency
        Task<List<ExchangeRate>> GetByBaseCurrency(string baseCurrencySymbol);
        Task UpdateByBaseCurrency(string baseCurrencySymbol, List<ExchangeRate> exchangeRates);
        Task DeleteByBaseCurrency(string baseCurrencySymbol);
        #endregion ByBaseCurrency

        Task<decimal?> GetAverageRate(string baseCurrencySymbol,
            string targetCurrencySymbol, DateTime startDate,
            DateTime endDate);

    }
}
