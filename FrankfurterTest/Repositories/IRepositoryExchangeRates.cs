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
        Task<List<ExchangeRate>> GetByBaseCurrency(int baseCurrencyId);

    }
}
