using FrankfurterTest.DTOs;
using FrankfurterTest.Entities;
using FrankfurterTest.Utility;
using Microsoft.EntityFrameworkCore;

namespace FrankfurterTest.Repositories
{
    public class ExchangeRatesRepository :IRepositoryExchangeRates
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public ExchangeRatesRepository(ApplicationDbContext context, 
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }
        public async Task<List<ExchangeRate>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.ExchangesRates.AsQueryable();
            await httpContext.InsertParamsPag(queryable);
            return await queryable.OrderBy(x => x.Id)
                .Include(e => e.BaseCurrency)
                .Include(e => e.TargetCurrency).Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<ExchangeRate?> GetById(int id)
        {
            return await context.ExchangesRates
                .Include(e => e.BaseCurrency)
                .Include(e => e.TargetCurrency)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task Update(ExchangeRate exchangeRate)
        {
            context.ExchangesRates.Update(exchangeRate);
            await context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var exchangeRate = await context.ExchangesRates.FindAsync(id);
            if (exchangeRate != null)
            {
                context.ExchangesRates.Remove(exchangeRate);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ExchangeRate>> GetByBaseCurrency(int baseCurrencyId)
        {
            return await context.ExchangesRates
                .Include(e => e.BaseCurrency)
                .Include(e => e.TargetCurrency)
                .Where(e => e.BaseCurrencyId == baseCurrencyId)
                .ToListAsync();
        }

        public async Task<int> CreateExchangeRate(ExchangeRate rate)
        {
            context.Add(rate);
            await context.SaveChangesAsync();
            return rate.Id;
        }

    }
}
