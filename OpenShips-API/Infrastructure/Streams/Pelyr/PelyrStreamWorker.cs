using System.Net.WebSockets;
using OpenShipsAPI.Infrastructure.Streams.Common;

namespace OpenShipsAPI.Infrastructure.Streams.Pelyr;

public class PelyrStreamWorker(
//StreamChannel channel,
    AisMessageDispatcher dispatcher,
    IConfiguration configuration,
    ILogger<PelyrStreamWorker> logger)
    : WebSocketStreamWorker(
    //channel,
    dispatcher,
    logger)
{
    protected override AisSource Source => AisSource.Pelyr;

    protected override string Url =>
        configuration["Streams:Pelyr"]!;

    protected override Task ConfigureSocketAsync(ClientWebSocket socket, CancellationToken cancellationToken)
    {
        // Configure the WebSocket options
        var token =
            configuration["ApiKeys:Pelyr"]!;

        socket.Options.SetRequestHeader("Authorization", $"Bearer {token}");

        return Task.CompletedTask;
    }
    
    protected override async Task OnConnectedAsync(
        ClientWebSocket socket,
        CancellationToken cancellationToken)
    {
        var subscription = new
        {
            type = "subscribe",
            id = "ais",
            fields = "full",
            include_positionless = true,
            msg_types = new[] {1, 2, 3, 4, 9, 18, 19, 21}
        };

        await SendJsonAsync(
            socket,
            subscription,
            cancellationToken);
    }
}