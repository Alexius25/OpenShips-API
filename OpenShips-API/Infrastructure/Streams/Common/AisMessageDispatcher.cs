using System.Collections.Concurrent;
using System.Text.Json;
using OpenShipsAPI.Domain.Events;

namespace OpenShipsAPI.Infrastructure.Streams.Common;

public class AisMessageDispatcher(
    IEnumerable<IAisMessageHandler> handlers,
    ILogger<AisMessageDispatcher> logger)
{
    
    public static readonly ConcurrentDictionary<(AisSource Source, int MessageType), ulong>
        MessageCounts = new();
    
    public static readonly ConcurrentDictionary<AisSource, DateTimeOffset>
        LastMessageReceived = new();
    
    public AisEvent? Dispatch(
        JsonElement message,
        AisSource source)
    {
        if (!AisUtils.TryGetMessageType(message, logger, source, out var messageType))
            return null;

        MessageCounts.AddOrUpdate(
            (source, messageType),
            1UL,
            (_, count) => count + 1UL);
        
        LastMessageReceived[source] = DateTimeOffset.UtcNow;
        
        
        foreach (var handler in handlers)
        {
            if (handler.CanHandle(message, source))
                return handler.HandleMessage(message, source);
        }

        return null;
    }
}