using AutoMapper;
using FrankfurterTest.DTOs;
using FrankfurterTest.Entities;
using FrankfurterTest.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

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

    }
}
