using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;

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

    public async Task<AggregatedResponse> GetAggregatedDataAsync(AggregationRequest request)
    {
        var weatherTask = _weatherService.GetWeatherAsync(request.City);
        var newsTask = _hackerNewsService.GetItemByIdAsync(request.HackerNewsItemId);
        var countryTask = _worldBankService.GetCountryAsync(request.CountryCode);

        await Task.WhenAll(weatherTask, newsTask, countryTask);

        var items = new List<AggregatedItem>
        {
            new AggregatedItem
            {
                Source = "Weather",
                Category = weatherTask.Result.Category,
                Date = weatherTask.Result.Timestamp,
                RawData = weatherTask.Result
            },
            new AggregatedItem
            {
                Source = "HackerNews",
                Category = newsTask.Result.Category,
                Date = newsTask.Result.CreatedAt,
                RawData = newsTask.Result
            },
            new AggregatedItem
            {
                Source = "WorldBank",
                Category = countryTask.Result.Category,
                Date = countryTask.Result.LastUpdated,
                RawData = countryTask.Result
            }
        };

        // Filtering
        List<AggregatedItem> filtered = items
            .Where(i => string.IsNullOrEmpty(request.Category) || i.Category == request.Category)
            .ToList();

        // Sorting
        filtered = request.SortBy?.ToLower() switch
        {
            "date" => request.SortOrder == "desc"
                ? filtered.OrderByDescending(i => i.Date).ToList()
                : filtered.OrderBy(i => i.Date).ToList(),
            _ => filtered
        };

        return new AggregatedResponse(filtered);
    }
}
