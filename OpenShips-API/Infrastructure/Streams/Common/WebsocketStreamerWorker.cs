using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace OpenShipsAPI.Infrastructure.Streams.Common;

public abstract class WebSocketStreamWorker(
    //StreamChannel channel,
    AisMessageDispatcher dispatcher,
    ILogger logger)
    : BackgroundService
{
    protected abstract AisSource Source { get; }
    protected abstract string Url { get; }

    protected virtual Task ConfigureSocketAsync(
        ClientWebSocket socket,
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    protected virtual Task OnConnectedAsync(
        ClientWebSocket socket,
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    protected override async Task ExecuteAsync(
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await RunConnectionAsync(cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (WebSocketException ex)
            {
                logger.LogWarning(
                    ex,
                    "WebSocket {Source} disconnected. Reconnecting in 5 seconds.",
                    Source);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "WebSocket {Source} failed unexpectedly.",
                    Source);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    cancellationToken);
            }
        }
    }

    private async Task RunConnectionAsync(
        CancellationToken cancellationToken)
    {
        using var socket = new ClientWebSocket();

        await ConfigureSocketAsync(
            socket,
            cancellationToken);

        await socket.ConnectAsync(
            new Uri(Url),
            cancellationToken);

        logger.LogInformation(
            "Connected to WebSocket {Source}.",
            Source);

        await OnConnectedAsync(
            socket,
            cancellationToken);

        while (
            socket.State == WebSocketState.Open &&
            !cancellationToken.IsCancellationRequested)
        {
            var json = await ReceiveMessageAsync(
                socket,
                cancellationToken);

            if (json is null)
                break;

            await ProcessMessageAsync(
                json,
                cancellationToken);
        }
    }
    
    private Task ProcessMessageAsync(
        string json,
        CancellationToken cancellationToken)
    {
        try
        {
            using var document = JsonDocument.Parse(json);

            var result = dispatcher.Dispatch(
                document.RootElement,
                Source);

            if (result is null)
            {
                logger.LogDebug(
                    "Unknown message from {Source}: {Json}",
                    Source,
                    json);
            }

            return Task.CompletedTask;
        }
        catch (JsonException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid JSON from {Source}.",
                Source);

            return Task.CompletedTask;
        }
    }

    private static async Task<string?> ReceiveMessageAsync(
        ClientWebSocket socket,
        CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();

        var buffer = new byte[8192];

        while (true)
        {
            var result = await socket.ReceiveAsync(
                buffer,
                cancellationToken);

            if (result.MessageType ==
                WebSocketMessageType.Close)
            {
                return null;
            }

            stream.Write(
                buffer,
                0,
                result.Count);

            if (result.EndOfMessage)
                break;
        }

        return Encoding.UTF8.GetString(
            stream.ToArray());
    }

    protected static async Task SendJsonAsync(
        ClientWebSocket socket,
        object message,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(message);

        var bytes = Encoding.UTF8.GetBytes(json);

        await socket.SendAsync(
            bytes,
            WebSocketMessageType.Text,
            true,
            cancellationToken);
    }
}