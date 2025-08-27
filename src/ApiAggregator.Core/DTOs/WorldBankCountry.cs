namespace ApiAggregator.Core.DTOs;

public record WorldBankCountry(
    string Id,
    string Iso2Code,
    string Name,
    string Region,
    string AdminRegion,
    string IncomeLevel,
    string LendingType,
    string CapitalCity,
    double Latitude,
    double Longitude
);