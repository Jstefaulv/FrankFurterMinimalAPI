using AutoMapper;
using FrankfurterTest.DTOs;
using FrankfurterTest.Entities;
using FrankfurterTest.Repositories;
using FrankfurterTest.Services;
using Microsoft.AspNetCore.Http.HttpResults;
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

    }
}
