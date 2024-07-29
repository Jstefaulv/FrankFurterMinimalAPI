using AutoMapper;
using FrankfurterTest.DTOs;
using FrankfurterTest.Entities;

namespace FrankfurterTest.Utility
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CreateCurrencyDTO, Currency>();
            CreateMap<Currency, CurrencyDTO>();

            CreateMap<CreateExchangeRateDTO, ExchangeRate>();
            CreateMap<ExchangeRate, ExchangeRateDTO>();
        }
    }
}
