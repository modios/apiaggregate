using ApiAggregator.Core.Interfaces;
using ApiAggregator.Core.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IRequestStatsStore _statsStore;

    public StatsController(IRequestStatsStore statsStore)
    {
        _statsStore = statsStore;
    }

    [HttpGet]
    public ActionResult<Dictionary<string, RequestStats>> GetStats()
    {
        return Ok(_statsStore.GetStats());
    }
}
