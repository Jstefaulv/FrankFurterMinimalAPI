using FrankfurterTest.DTOs;
using FrankfurterTest.Entities;

namespace FrankfurterTest.Repositories
{
    public interface IRepositoryCurrencies
    {
        Task<int> CreateCurrency(Currency currency);
        Task <Currency?> GetById (int id);
        Task<List<Currency>> GetAll(PaginationDTO paginationDTO);
        Task Update(Currency currency);
        Task<bool> Exist(int id);
        Task Delete(int id);
        Task<List<Currency>> GetBySymbol(string symbol);
    }
}
