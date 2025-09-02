using ApiAggregator.API.Constants;
using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[EnableRateLimiting(RateLimitingPolicies.FixedPolicy)]
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AggregationController : ControllerBase
{
    private readonly IAggregationService _aggregationService;
    private readonly ILogger<AggregationController> _logger;

    public AggregationController(IAggregationService aggregationService, ILogger<AggregationController> logger)
    {
        _aggregationService = aggregationService;
        _logger = logger;
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _aggregationService.GetAggregatedDataAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid input for aggregation request: {@Request}", request);
            return BadRequest(ex.Message);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "External API call failed during aggregation for request: {@Request}", request);
            return StatusCode(503, "One or more external services are unavailable.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during aggregation for request: {@Request}", request);
            return StatusCode(500, "An unexpected error occurred while processing your request.");
        }
    }
}
