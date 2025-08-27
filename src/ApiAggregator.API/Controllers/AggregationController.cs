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

    [HttpGet]
    public async Task<ActionResult<AggregatedResponse>> GetAggregated(
        [FromQuery] string city,
        [FromQuery] int hackerNewsItemId,
        [FromQuery] string countryCode)
    {
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(countryCode))
            return BadRequest("City and countryCode are required.");

        var result = await _aggregationService.GetAggregatedDataAsync(city, hackerNewsItemId, countryCode);
        return Ok(result);
    }
}