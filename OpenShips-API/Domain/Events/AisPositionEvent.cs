using OpenShipsAPI.Infrastructure.Streams.Common;

namespace OpenShipsAPI.Domain.Events;

public class AisPositionEvent : AisEvent
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    
    // Main non position fields
    public double? Sog { get; init; }
    public double? Cog { get; init; }
    public int? Heading { get; init; }
    public NavigationStatus? NavigationStatus { get; init; }
    public double? RateOfTurn { get; init; }
    public bool? PositionAccuracy { get; init; }
    
    // AisStream specific types
    public int RepeatIndicator { get; set; }
    public bool Valid { get; set; }
    public int SpecialManoeuvreIndicator { get; set; }
    public int Spare { get; set; }
    public bool Raim { get; set; }
    public int CommunicationState { get; set; }
}