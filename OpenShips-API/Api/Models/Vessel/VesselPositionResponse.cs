using OpenShipsAPI.Domain.Events;
using OpenShipsAPI.Infrastructure.Streams.Common;

namespace OpenShipsAPI.Api.Models.Vessel;

public class VesselPositionResponse
{
    public AisSource Source { get; init; }
    public AisDataLicense License  { get; init; }
    public int MessageType { get; init; }
    public int Mmsi { get; init; }
    public DateTimeOffset ReceivedAt { get; init; }
    public DateTimeOffset EventTimestamp { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? ShipName { get; init; }
    public int? ShipType { get; init; } 
    public double? Sog { get; init; }
    public double? Cog { get; init; }
    public int? Heading { get; init; }
    public NavigationStatus? NavigationStatus { get; init; }
    public double? RateOfTurn { get; init; }
    public bool? PositionAccuracy { get; init; }
    public int RepeatIndicator { get; set; }
    public bool Valid { get; set; }
    public int SpecialManoeuvreIndicator { get; set; }
    public int Spare { get; set; }
    public bool Raim { get; set; }
    public int CommunicationState { get; set; }
}