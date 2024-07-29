using FrankfurterTest.DTOs;
using FrankfurterTest.Entities;
using FrankfurterTest.Utility;
using Microsoft.EntityFrameworkCore;

namespace FrankfurterTest.Repositories
{
    public class CurrencyRepository : IRepositoryCurrencies
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;
        public CurrencyRepository(ApplicationDbContext context, 
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;

        }
        public async Task<int> CreateCurrency(Currency currency)
        {
            context.Add(currency);
            await context.SaveChangesAsync();
            return currency.Id;
        }


        public async Task<List<Currency>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Currencies.AsQueryable();
            await httpContext.InsertParamsPag(queryable);
            return await queryable.OrderBy(x => x.Symbol).Paginate(paginationDTO).ToListAsync();
        }

        public async Task<Currency?> GetById(int id)
        {
            return await context.Currencies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<bool> Exist(int id)
        {
            return await context.Currencies.AnyAsync(x => x.Id == id);
        }
        public async Task Update(Currency currency)
        {
            context.Update(currency);
            await context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await context.Currencies.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<List<Currency>> GetBySymbol(string symbol)
        {
            return await context.Currencies.Where(c => c.Symbol.Contains(symbol))
                .OrderBy(c => c.Symbol).ToListAsync();

        }
    }
}
