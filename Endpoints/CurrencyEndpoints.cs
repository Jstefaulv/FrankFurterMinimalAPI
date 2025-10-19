using AutoMapper;
using FrankfurterTest.DTOs;
using FrankfurterTest.Entities;
using FrankfurterTest.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;


namespace FrankfurterTest.Endpoints
{
    public static class CurrencyEndpoints
    {
       public static RouteGroupBuilder MapCurrencies(this RouteGroupBuilder gr)
        {

            gr.MapPost("/", CreateCurrency);

            gr.MapGet("/", GetCurrencies)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("currencies-get"));

            gr.MapGet("getbysymbol/{symbol}",GetCurrenciesBySymbol);
            gr.MapGet("/{id:int}", GetCurrencyById);

            gr.MapPut("/{id:int}", UpdateCurrency);

            gr.MapDelete("/{id:int}", DeleteCurrencies);

            return gr;
        }


        static async Task<Ok<List<CurrencyDTO>>> GetCurrencies(IRepositoryCurrencies repo, 
            IMapper mapper, int page = 1, int recordsByPage=10)
        {
            var pagination = new PaginationDTO {Page = page,RecordsByPage= recordsByPage };
            var currencies = await repo.GetAll(pagination);
            var currenciesDTO = mapper.Map<List<CurrencyDTO>>(currencies);
            return TypedResults.Ok(currenciesDTO);
        }

        static async Task<Results<Ok<CurrencyDTO>, NotFound>> GetCurrencyById(IRepositoryCurrencies repo,
            int id, IMapper mapper)
        {
            var currency = await repo.GetById(id);

            if (currency is null)
            {
                return TypedResults.NotFound();
            }
            var currencyDTO = mapper.Map<CurrencyDTO>(currency);
            return TypedResults.Ok(currencyDTO);
        }

        static async Task<Created<CurrencyDTO>> CreateCurrency(CreateCurrencyDTO createCurrencyDTO, IRepositoryCurrencies repo,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var currency = mapper.Map<Currency>(createCurrencyDTO);
            var id = await repo.CreateCurrency(currency);
            await outputCacheStore.EvictByTagAsync("currencies-get", default);
            var currencyDTO = mapper.Map<CurrencyDTO>(currency);

            return TypedResults.Created($"/currencies/{id}", currencyDTO);
        }

        static async Task<Results<NoContent, NotFound>> UpdateCurrency(int id, CreateCurrencyDTO createCurrencyDTO, IRepositoryCurrencies repo,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var exist = await repo.Exist(id);

            if (!exist)
            {
                return TypedResults.NotFound();
            }
            var currency = mapper.Map<Currency>(createCurrencyDTO);
            currency.Id = id;

            await repo.Update(currency);
            await outputCacheStore.EvictByTagAsync("currencies-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> DeleteCurrencies(int id, IRepositoryCurrencies repo,
            IOutputCacheStore outputCacheStore)
        {
            var exist = await repo.Exist(id);

            if (!exist)
            {
                return TypedResults.NotFound();
            }

            await repo.Delete(id);
            await outputCacheStore.EvictByTagAsync("currencies-get", default);
            return TypedResults.NoContent();

        }

        static async Task<Ok<List<CurrencyDTO>>> GetCurrenciesBySymbol(string symbol, IRepositoryCurrencies repo, IMapper mapper)
        {
            var currencies = await repo.GetBySymbol(symbol);
            var currenciesDTO = mapper.Map<List<CurrencyDTO>>(currencies);
            return TypedResults.Ok(currenciesDTO);
        }
    }
}
