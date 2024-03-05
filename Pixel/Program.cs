using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pixel;
using Pixel.Shared.Contracts;
using Pixel.Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddTransient<RedisPublisher>();
var app = builder.Build();

app.MapGet("/track", async (HttpRequest request,
    [FromServices] RedisPublisher publisher,
    [FromServices] IOptions<RedisOptions> redisOptions) =>
{
    const string content = "R0lGODdhAQABAIEAAP///wAAAAAAAAAAACwAAAAAAQABAAAIBAABBAQAOw==";
    var redisConfiguration = redisOptions.Value;
    
    var message = new TrackerRecord(
        DateTimeOffset.UtcNow,
        request.Headers[HeaderNames.UserAgent].FirstOrDefault(),
        request.Headers[HeaderNames.Referer].FirstOrDefault(),
        GetUserIp(request));

    await publisher.PublishAsync(redisConfiguration.TrackerRecordsChannel,
        JsonSerializer.Serialize(message));
    return Results.File(Encoding.UTF8.GetBytes(content), "image/gif");
});

app.Run();
return;

string GetUserIp(HttpRequest request)
{
    var requestIpAddress = request.Headers[HeaderNames.ForwardedFrom].FirstOrDefault();
    if (string.IsNullOrEmpty(requestIpAddress))
    {
        requestIpAddress = request.HttpContext.Connection.RemoteIpAddress!.ToString();
    }

    return requestIpAddress;
}