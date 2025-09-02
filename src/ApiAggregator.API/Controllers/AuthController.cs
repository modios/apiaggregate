using ApiAggregator.Core.DTOs;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(JwtTokenService jwtTokenService, ILogger<AuthController> logger)
    {
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // TODO: Replace with real user validation
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            _logger.LogWarning("Login attempt with empty username.");
            return BadRequest("Username is required.");
        }

        // Simulate successful login
        var token = _jwtTokenService.GenerateToken(request.Username);
        _logger.LogInformation("User {Username} logged in successfully.", request.Username);

        return Ok(new { token });
    }
}
