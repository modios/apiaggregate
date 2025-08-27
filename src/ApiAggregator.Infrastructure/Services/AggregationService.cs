using ApiAggregator.Core.Interfaces;

namespace ApiAggregator.Infrastructure.Services;

public class AggregationService : IAggregationService
{
    private readonly IOpenMeteoService _weatherService;
    private readonly IHackerNewsService _hackerNewsService;
    private readonly IWorldBankCountryService _worldBankService;

    public AggregationService(
        IOpenMeteoService weatherService,
        IHackerNewsService hackerNewsService,
        IWorldBankCountryService worldBankService)
    {
        _weatherService = weatherService;
        _hackerNewsService = hackerNewsService;
        _worldBankService = worldBankService;
    }

    public async Task<AggregatedResponse> GetAggregatedDataAsync(string city, int hackerNewsItemId, string countryCode)
    {
        var weatherTask = _weatherService.GetWeatherAsync(city);
        var newsTask = _hackerNewsService.GetItemByIdAsync(hackerNewsItemId);
        var countryTask = _worldBankService.GetCountryAsync(countryCode);

        await Task.WhenAll(weatherTask, newsTask, countryTask);

        return new AggregatedResponse(
            Weather: weatherTask.Result,
            HackerNewsItem: newsTask.Result,
            WorldBankCountry: countryTask.Result!
        );
    }
}
