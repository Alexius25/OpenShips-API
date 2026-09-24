using OpenShipsAPI.Infrastructure.Streams.Common;

namespace OpenShipsAPI.Domain.Events;

public abstract class AisEvent
{
    public AisSource Source { get; init; }
    public string? SourceMessageId { get; init; }
    public AisDataLicense License  { get; init; }
    
    public int MessageType { get; init; }
    public int Mmsi { get; init; }
    public DateTimeOffset ReceivedAt { get; init; }
    public DateTimeOffset EventTimestamp { get; init; }
}