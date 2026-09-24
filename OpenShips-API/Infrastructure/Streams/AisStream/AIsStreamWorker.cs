using System.Net.WebSockets;
using OpenShipsAPI.Infrastructure.Streams.Common;

namespace OpenShipsAPI.Infrastructure.Streams.AisStream;

public class AisStreamWorker(
    //StreamChannel channel,
    AisMessageDispatcher dispatcher,
    IConfiguration configuration,
    ILogger<AisStreamWorker> logger)
    : WebSocketStreamWorker(
        // channel,
        dispatcher,
        logger)
{
    protected override AisSource Source => AisSource.AisStream;
    protected override string Url => configuration["Streams:AisStream"];

    protected override async Task OnConnectedAsync(
        ClientWebSocket socket,
        CancellationToken cancellationToken)
    {
        var apiKey = configuration["ApiKeys:AisStream"];
        
        await SendJsonAsync(
            socket,
            new
            {
                APIKey = apiKey,

                BoundingBoxes = new[]
                {
                    new[]
                    {
                        new[] { 90.0, -180.0 },
                        new[] { -90.0, 180.0 }
                    }
                },
                
                FilterMessageTypes = new[]
                {
                    "PositionReport",
                    "ShipStaticData",
                    "BaseStationReport"
                }
            },
            cancellationToken);
    }
}