using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Enums;
using ApiAggregator.Core.Interfaces;
using Microsoft.Extensions.Logging;

public class AggregationService : IAggregationService
{
    private readonly IOpenMeteoService _weatherService;
    private readonly IHackerNewsService _hackerNewsService;
    private readonly IWorldBankCountryService _worldBankService;
    private readonly ILogger<AggregationService> _logger;

    public AggregationService(
        IOpenMeteoService weatherService,
        IHackerNewsService hackerNewsService,
        IWorldBankCountryService worldBankService,
        ILogger<AggregationService> logger)
    {
        _weatherService = weatherService;
        _hackerNewsService = hackerNewsService;
        _worldBankService = worldBankService;
        _logger = logger;
    }

    public async Task<AggregatedResponse> GetAggregatedDataAsync(AggregationRequest request)
    {
        try
        {
            var weatherTask = _weatherService.GetWeatherAsync(request.City);
            var newsTask = _hackerNewsService.GetItemByIdAsync(request.HackerNewsItemId);
            var countryTask = _worldBankService.GetCountryAsync(request.CountryCode);

            await Task.WhenAll(weatherTask, newsTask, countryTask);

            var items = BuildAggregatedItems(weatherTask.Result, newsTask.Result, countryTask.Result);
            var filtered = FilterItems(items, request.Category);
            var sorted = SortItems(filtered, request.SortBy, request.SortOrder);

            return new AggregatedResponse(sorted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error aggregating data for request {@Request}", request);
            throw;
        }
    }
    
    private List<AggregatedItem> BuildAggregatedItems(
            WeatherInfo weather,
            HackerNewsItem news,
            WorldBankCountry country)
    {
        return new List<AggregatedItem>
        {
            new AggregatedItem
            {
                Source = Category.Weather.ToString(),
                Category = weather.Category,
                Date = weather.Timestamp,
                RawData = weather
            },
            new AggregatedItem
            {
                Source = Category.HackerNews.ToString(),
                Category = news.Category,
                Date = news.CreatedAt,
                RawData = news
            },
            new AggregatedItem
            {
                Source = Category.WorldBank.ToString(),
                Category = country.Category,
                Date = country.LastUpdated,
                RawData = country
            }
        };
    }

    private List<AggregatedItem> FilterItems(List<AggregatedItem> items, Category? category)
    {
        return items
            .Where(i => !category.HasValue || i.Category == category.Value)
            .ToList();
    }

    private List<AggregatedItem> SortItems(List<AggregatedItem> items, SortBy? sortBy, SortOrder sortOrder)
    {
        return sortBy switch
        {
            SortBy.Date => sortOrder == SortOrder.Desc
                ? items.OrderByDescending(i => i.Date).ToList()
                : items.OrderBy(i => i.Date).ToList(),
            _ => items
        };
    }
}
