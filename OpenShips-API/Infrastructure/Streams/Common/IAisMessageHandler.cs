using System.Text.Json;
using OpenShipsAPI.Domain.Events;

namespace OpenShipsAPI.Infrastructure.Streams.Common;

public interface IAisMessageHandler
{
    bool CanHandle(JsonElement message, AisSource source);
    
    AisEvent? HandleMessage(JsonElement message, AisSource source);
}