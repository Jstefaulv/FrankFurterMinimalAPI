using FrankfurterTest.DTOs;
using FrankfurterTest.Entities;
using FrankfurterTest.Services;
using FrankfurterTest.Utility;
using Microsoft.EntityFrameworkCore;
using System.Formats.Tar;

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


        public async Task<int> CreateExchangeRate(ExchangeRate rate)
        {
            context.Add(rate);
            await context.SaveChangesAsync();
            return rate.Id;
        }

        public async Task<List<ExchangeRate>> GetByBaseCurrency(string baseCurrencySymbol)
        {
            return await context.ExchangesRates
                .Include(e => e.BaseCurrency)
                .Include(e => e.TargetCurrency)
                .Where(e => e.BaseCurrency.Symbol == baseCurrencySymbol)
                .ToListAsync();      
        }

        public async Task UpdateByBaseCurrency(string baseCurrencySymbol, List<ExchangeRate> exchangeRates)
        {
            var existingRates = await context.ExchangesRates
                 .Where(e => e.BaseCurrency.Symbol == baseCurrencySymbol)
                 .ToListAsync();

            context.ExchangesRates.RemoveRange(existingRates);
            context.ExchangesRates.AddRange(exchangeRates);
            await context.SaveChangesAsync();
        }

        public async Task DeleteByBaseCurrency(string baseCurrencySymbol)
        {
            var exchangeRates = await context.ExchangesRates
                .Where(e => e.BaseCurrency.Symbol == baseCurrencySymbol)
                .ToListAsync();

            context.ExchangesRates.RemoveRange(exchangeRates);
            await context.SaveChangesAsync();
        }

        public async Task<decimal?> GetAverageRate(string baseCurrencySymbol, 
            string targetCurrencySymbol, DateTime startDate, DateTime endDate)
        {
            var rates = await context.ExchangesRates
                .Where(e => e.BaseCurrency.Symbol == baseCurrencySymbol
                && e.TargetCurrency.Symbol == targetCurrencySymbol
                && e.Date >= startDate && e.Date <= endDate)
                .ToListAsync();

            if (!rates.Any())
            {
                return null;
            }

            return rates.Average(e => e.Rate);
                
        }


    }
}
