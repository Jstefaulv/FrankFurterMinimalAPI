using AutoMapper;
using FrankfurterTest.DTOs;
using FrankfurterTest.Entities;
using FrankfurterTest.Repositories;
using FrankfurterTest.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace FrankfurterTest.Endpoints
{
    public static class ExchangeRateEndpoints
    {
        public static RouteGroupBuilder MapExchangeRates(this RouteGroupBuilder gr)
        {
            gr.MapGet("/", GetExchangeRates)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("rates-get"));
            
            gr.MapGet("{id:int}", GetRateById);


            gr.MapPost("/", CreateRate);

            gr.MapPut("/{id:int}", UpdateRates);
            gr.MapDelete("/{id:int}", DeleteRates);

            gr.MapPut("/update", UpdateRatesFR);

            gr.MapGet("/currency/{baseCurrency}", GetRatesByBaseCurrency)
                 .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("rates-get"));
            gr.MapPut("/currency/{baseCurrency}", UpdateRatesByBaseCurrency);
            gr.MapDelete("/currency/{baseCurrency}", DeleteRatesByBaseCurrency);

            gr.MapGet("/average", GetAverageRates)
                 .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("rates-get"));
            gr.MapGet("/minmax", GetMinMaxExchangeRate)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("rates-get"));
            return gr;
        }

        static async Task<Ok<List<ExchangeRateDTO>>> GetExchangeRates(IRepositoryExchangeRates repo,
            IMapper mapper, int page = 1, int recordsByPage = 10)
        {
            var pagination = new PaginationDTO { Page = page, RecordsByPage = recordsByPage };
            var exchangeRates = await repo.GetAll(pagination);
            var exchangeRateDTO = mapper.Map<List<ExchangeRateDTO>>(exchangeRates);
            return TypedResults.Ok(exchangeRateDTO);
        }

        static async Task<Results<Ok<ExchangeRateDTO>, NotFound>> GetRateById(IRepositoryExchangeRates repo,
            int id, IMapper mapper)
        {
            var rate = await repo.GetById(id);

            if (rate is null)
            {
                return TypedResults.NotFound();
            }
            var rateDTO = mapper.Map<ExchangeRateDTO>(rate);
            return TypedResults.Ok(rateDTO);
        }

        static async Task<Created<ExchangeRateDTO>> CreateRate(CreateExchangeRateDTO createRateDTO, 
            IRepositoryExchangeRates repo, 
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var rate = mapper.Map<ExchangeRate>(createRateDTO);
            var id = await repo.CreateExchangeRate(rate);
            await outputCacheStore.EvictByTagAsync("rates-get", default);
            var rateDTO = mapper.Map<ExchangeRateDTO>(rate);

            return TypedResults.Created($"/rates/{id}", rateDTO);
        }

        static async Task<Results<NotFound,NoContent>> UpdateRates(int id, 
            CreateExchangeRateDTO createRateDTO, IRepositoryExchangeRates repo,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var rates = await repo.GetById(id);
            if (rates == null) {
                return TypedResults.NotFound();
            }
            var rate = mapper.Map<ExchangeRate>(createRateDTO);
            rate.Id = id;

            await repo.Update(rate);
            await outputCacheStore.EvictByTagAsync("rates-get", default);
            return TypedResults.NoContent();

        }

        public static async Task<Results<NoContent,NotFound>> DeleteRates(int id, 
            IRepositoryExchangeRates repo, IOutputCacheStore outputCacheStore)
        {
            var rate = await repo.GetById(id);
            if ( rate == null)
            {
                return TypedResults.NotFound();
            }

            await repo.Delete(id);
            await outputCacheStore.EvictByTagAsync("rates-get", default);
            return TypedResults.NoContent();
        }

        static async Task<IResult> UpdateRatesFR(FrankfurterService fService, 
            ApplicationDbContext context, string baseCurrency="EUR", string targetCurrency = "USD")
        {

            var exchangeRateResponse = await fService.GetExchangeRatesAsync(baseCurrency,targetCurrency);

            if(exchangeRateResponse == null)
            {
                return Results.Problem("Error fetching exchange rates from Frankfurter");
            }

            var baseCurrencyEntity = await context.Currencies.
                FirstOrDefaultAsync(c => c.Symbol == baseCurrency);
            if (baseCurrencyEntity == null)
            {
                return Results.Problem("Error: BaseCurrency Not Found");
            }

            var targetCurrencyEntity = await context.Currencies.
                FirstOrDefaultAsync(c => c.Symbol == targetCurrency);

            if (targetCurrencyEntity ==null)
            {
                return Results.Problem("Error: TargetCurrency Not Found");
            }

            if (!exchangeRateResponse.Rates.TryGetValue(targetCurrency, out var rate) ||
                rate == 0)
            {
                return Results.Problem("Invalid Exchange Rate from Frankfurter");
            }


            var exchangeRate = new ExchangeRate
            {
                BaseCurrencyId = baseCurrencyEntity.Id,
                TargetCurrencyId = targetCurrencyEntity.Id,
                Rate = rate,
                Date = exchangeRateResponse.Date
            };

            context.ExchangesRates.Add(exchangeRate);
            await context.SaveChangesAsync();

            return Results.Ok(exchangeRate);
        }

        static async Task<IResult> GetRatesByBaseCurrency(string baseCurrency,
            IRepositoryExchangeRates repo, IMapper mapper)
        {
            var exchangeRates = await repo.GetByBaseCurrency(baseCurrency);
            return Results.Ok(exchangeRates);
        }

        static async Task<IResult> UpdateRatesByBaseCurrency(string baseCurrency,
            IRepositoryExchangeRates repo,
            List<ExchangeRateDTO> rateDTOs, ApplicationDbContext context)
        {
            var baseCurrencyEntity = await context.Currencies.
                FirstOrDefaultAsync(c => c.Symbol == baseCurrency);   

            if(baseCurrencyEntity == null)
            {
                return Results.Problem($"Base Currency {baseCurrency} Not Found.");
            }

            var exchangeRates = rateDTOs.Select(dto => new ExchangeRate
            {
                BaseCurrencyId = baseCurrencyEntity.Id,
                TargetCurrencyId = dto.TargetCurrencyId,
                Rate = dto.Rate,
                Date = dto.Date
            }).ToList();

            await repo.UpdateByBaseCurrency(baseCurrency, exchangeRates);
            return Results.NoContent();

        }

        static async Task<IResult> DeleteRatesByBaseCurrency(string baseCurrency,
            IRepositoryExchangeRates repo)
        {
            await repo.DeleteByBaseCurrency(baseCurrency);
            return Results.NoContent();
        }

        static async Task<IResult> GetAverageRates(FrankfurterService frService,
            IRepositoryExchangeRates repo,
            string baseCurrency, string targetCurrency, DateTime start, DateTime end)
        {
            var exchangeRateResponse = await frService.GetExchangeRatesInRangeAsync(baseCurrency, targetCurrency, start, end);

            if (exchangeRateResponse?.Rates == null || !exchangeRateResponse.Rates.Any())
            {
                return Results.NotFound("No Exchange Rates Found");
            }

            var rates = exchangeRateResponse.Rates
                .SelectMany(day => day.Value.ContainsKey(targetCurrency) ? new[] { day.Value[targetCurrency] } : Array.Empty<decimal>())
                .ToList();

            if (!rates.Any())
            {
                return Results.NotFound("No Valid Exchange Rates Found");
            }

            var averageRate = rates.Average();
            return Results.Ok(averageRate);
        }

        public static async Task<IResult> GetMinMaxExchangeRate(FrankfurterService frService,
            string baseCurrency, string targetCurrency, DateTime startDate, DateTime endDate)
        {
            var exchangeRateResponse = await frService.GetExchangeRatesInRangeAsync(baseCurrency, targetCurrency, startDate, endDate);

            if (exchangeRateResponse?.Rates == null || !exchangeRateResponse.Rates.Any())
            {
                return Results.NotFound("No Exchange Rates Found");
            }

            var rates = exchangeRateResponse.Rates
                .SelectMany(day => day.Value.ContainsKey(targetCurrency) ? new[] { day.Value[targetCurrency] } : Array.Empty<decimal>())
                .ToList();

            if (!rates.Any())
            {
                return Results.NotFound("No Valid Exchange Rates Found.");
            }

            var minRate = rates.Min();
            var maxRate = rates.Max();

            return Results.Ok(new { MinRate = minRate, MaxRate = maxRate });
        }

    }
}
