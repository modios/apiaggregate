using ApiAggregator.Core.DTOs;
using ApiAggregator.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WorldBankCountryController : ControllerBase
{
    private readonly IWorldBankCountryService _service;

    public WorldBankCountryController(IWorldBankCountryService service)
    {
        _service = service;
    }

    [HttpGet("{iso2Code}")]
    public async Task<ActionResult<WorldBankCountry>> GetCountry(string iso2Code)
    {
        var country = await _service.GetCountryAsync(iso2Code.ToUpper());
        if (country is null)
            return NotFound("Country not found.");

        return Ok(country);
    }
}