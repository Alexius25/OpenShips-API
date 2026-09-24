using OpenShipsAPI.Domain.Destination;

namespace OpenShipsAPI.Api.Models.Vessel;

public class VesselDestination
{
    public long Id { get; set; }
    public DestinationType DestinationType { get; set; }
    public string ToName  { get; set; } = string.Empty;
    public int? ToPortId { get; set; }
    public string? FromName  { get; set; }
    public int? FromPortId { get; set; }
    public DateTimeOffset FirstSeen { get; set; }
    public DateTimeOffset LastSeen { get; set; }
}