using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;

public class WorldBankCountryService : IWorldBankCountryService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WorldBankCountryService> _logger;

    public WorldBankCountryService(HttpClient httpClient, ILogger<WorldBankCountryService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WorldBankCountry> GetCountryAsync(string iso2Code)
    {
        var url = $"https://api.worldbank.org/v2/country/{iso2Code}?format=json";

        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var countryElement = doc.RootElement[1][0];

            return new WorldBankCountry(
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
