using OpenShipsAPI.Infrastructure.Database.Models;

namespace OpenShipsAPI.Domain.Destination;

public class ShipDestination
{
    public long Id { get; set; }

    public int Mmsi { get; set; }

    public string Raw { get; set; } = null!;

    public DestinationType RouteType { get; set; }

    public string? From { get; set; }
    public string? To { get; set; }
    
    public int? ToPortId { get; set; }
    public PortDb? ToPort { get; set; }
    
    public int? FromPortId { get; set; }
    public PortDb? FromPort { get; set; }
    
    public DestinationMatchedBy? MatchedBy { get; set; }
    
    public DateTimeOffset FirstSeen { get; set; }
    public DateTimeOffset LastSeen { get; set; }
}