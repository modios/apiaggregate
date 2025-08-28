using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AggregationController : ControllerBase
{
    private readonly IAggregationService _aggregationService;

    public AggregationController(IAggregationService aggregationService)
    {
        _aggregationService = aggregationService;
    }

    /// <summary>
    /// Retrieves aggregated data from multiple external APIs.
    /// </summary>
    /// <param name="city">City name for weather data.</param>
    /// <param name="hackerNewsItemId">Item ID from Hacker News.</param>
    /// <param name="countryCode">Country code for news data.</param>
    /// <returns>Aggregated data response.</returns>
    [HttpGet]
    public async Task<ActionResult<AggregatedResponse>> GetAggregated([FromQuery] AggregationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.City) || string.IsNullOrWhiteSpace(request.CountryCode))
            return BadRequest("City and countryCode are required.");

        var result = await _aggregationService.GetAggregatedDataAsync(request);
        return Ok(result);
    }
}