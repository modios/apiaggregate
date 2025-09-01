using ApiAggregator.Core.DTOs;

namespace ApiAggregator.Core.Interfaces;

public interface IWorldBankCountryService
{
    Task<WorldBankCountry> GetCountryAsync(string iso2Code);
}