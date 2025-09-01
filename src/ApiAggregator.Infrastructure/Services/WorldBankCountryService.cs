using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace ApiAggregator.Infrastructure.Services;

public class WorldBankCountryService : IWorldBankCountryService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WorldBankCountryService> _logger;
    private readonly IMemoryCache _cache;

    public WorldBankCountryService(HttpClient httpClient, ILogger<WorldBankCountryService> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;
    }

    public async Task<WorldBankCountry> GetCountryAsync(string iso2Code)
    {
        string cacheKey = $"worldbank:country:{iso2Code.ToUpperInvariant()}";

        if (_cache.TryGetValue(cacheKey, out var cachedObj) && cachedObj is WorldBankCountry cachedCountry)
        {
            _logger.LogInformation("Returning cached World Bank country data for code {Iso2Code}", iso2Code);
            return cachedCountry;
        }

        var url = $"https://api.worldbank.org/v2/country/{iso2Code}?format=json";

        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var countryElement = doc.RootElement[1][0];

            var country = new WorldBankCountry(
                Id: countryElement.GetProperty("id").GetString() ?? "",
                Iso2Code: countryElement.GetProperty("iso2Code").GetString() ?? "",
                Name: countryElement.GetProperty("name").GetString() ?? "",
                Region: countryElement.GetProperty("region").GetProperty("value").GetString() ?? "",
                AdminRegion: countryElement.GetProperty("adminregion").GetProperty("value").GetString() ?? "",
                IncomeLevel: countryElement.GetProperty("incomeLevel").GetProperty("value").GetString() ?? "",
                LendingType: countryElement.GetProperty("lendingType").GetProperty("value").GetString() ?? "",
                CapitalCity: countryElement.GetProperty("capitalCity").GetString() ?? "",
                Latitude: double.Parse(countryElement.GetProperty("latitude").GetString() ?? "0"),
                Longitude: double.Parse(countryElement.GetProperty("longitude").GetString() ?? "0")
            );

            _cache.Set(cacheKey, country, TimeSpan.FromHours(1));
            return country;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for country code {Iso2Code} at URL {Url}", iso2Code, url);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing failed for World Bank country data for code {Iso2Code}.", iso2Code);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching World Bank country data for code {Iso2Code}.", iso2Code);
            throw;
        }
    }
}
