using Microsoft.AspNetCore.Mvc;
using OpenShipsAPI.Infrastructure.Streams.Common;

namespace OpenShipsAPI.Api.Controllers;

[ApiController]
[Route("/health")]
public class HealthController : ControllerBase
{
    private static readonly Dictionary<AisSource, TimeSpan> SourceTimeouts = new()
    {
        [AisSource.AisStream] = TimeSpan.FromSeconds(60),
        [AisSource.Pelyr] = TimeSpan.FromSeconds(30)
    };
    
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTimeOffset.UtcNow,
        });
    }

    [HttpGet("ais")]
    public IActionResult GetAis()
    {
        var now = DateTimeOffset.UtcNow;

        var sources = Enum
            .GetValues<AisSource>()
            .Where(source => source != AisSource.None)
            .ToDictionary(
                source => source.ToString(),
                source =>
                {
                    var hasReceived = 
                        AisMessageDispatcher.LastMessageReceived
                            .TryGetValue(source, out var lastReceived);

                    var healthy = hasReceived &&
                                  now - lastReceived <= SourceTimeouts[source];

                    return new
                    {
                        status = healthy ? "healthy" : "unhealthy",
                        lastMessage = hasReceived
                            ? lastReceived
                            : (DateTimeOffset?)null
                    };
                });

        var healthy = sources.Values.All(x => x.status == "healthy");

        return StatusCode(
            healthy
                ? StatusCodes.Status200OK
                : StatusCodes.Status503ServiceUnavailable,
            new
            {
                status = healthy ? "healthy" : "unhealthy",
                sources,
                timestamp = now
            });
    }
}
