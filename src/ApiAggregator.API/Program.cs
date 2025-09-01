using ApiAggregator.API.Constants;
using ApiAggregator.API.Policies;
using ApiAggregator.Core.Interfaces;
using ApiAggregator.Infrastructure.Services;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();


var resiliencePolicy = ResiliencePolicies.GetCombinedPolicy();

// Add services to the container.
builder.Host.UseSerilog();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IOpenMeteoService, OpenMeteoService>().AddPolicyHandler(resiliencePolicy); ;
builder.Services.AddHttpClient<IHackerNewsService, HackerNewsService>().AddPolicyHandler(resiliencePolicy); ;
builder.Services.AddHttpClient<IWorldBankCountryService, WorldBankCountryService>().AddPolicyHandler(resiliencePolicy);
builder.Services.AddScoped<IAggregationService, AggregationService>();


builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(RateLimitingPolicies.FixedPolicy, context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 5
            }));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();