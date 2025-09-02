namespace ApiAggregator.Core.Models;

public class JwtSettings
{
    public required string Issuer { get; set; } = default!;
    public required string Audience { get; set; } = default!;
    public required string Secret { get; set; } = default!;
}
