using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HackerNewsController : ControllerBase
{
    private readonly IHackerNewsService _service;

    public HackerNewsController(IHackerNewsService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HackerNewsItem>> GetItem(int id)
    {
        var item = await _service.GetItemByIdAsync(id);

        if (item == null)
        {
            return NotFound(); // Returns 404 with no body
        }

        return Ok(item); // Returns 200 with the HackerNewsItem serialized as JSON
    }
}
