using ApiAggregator.Core.DTOs;

public record AggregatedResponse(
    WeatherInfo Weather,
    HackerNewsItem HackerNewsItem,
    WorldBankCountry WorldBankCountry
);
