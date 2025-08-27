namespace ApiAggregator.Core.Interfaces;

using ApiAggregator.Core.DTOs;

public interface IHackerNewsService
{
    Task<HackerNewsItem> GetItemByIdAsync(int id);
}