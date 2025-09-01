using ApiAggregator.Core.Enums;

namespace ApiAggregator.Core.DTOs;

public record WorldBankCountry(
    string Id, string Iso2Code, string Name, string Region, string AdminRegion,
    string IncomeLevel, string LendingType, string CapitalCity, double Latitude, double Longitude)
{
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
    public Category Category => Category.WorldBank;
}
