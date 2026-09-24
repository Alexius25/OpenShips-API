using OpenShipsAPI.Infrastructure.Streams.Common;

namespace OpenShipsAPI.Infrastructure.Database.Models;

public class HistoricalAisPosition
{
    public long Id { get; set; }
    
    public AisSource Source { get; set; } = AisSource.None;
    public string? SourceMessageId { get; set; }
    public AisDataLicense License  { get; set; }
    
    public int MessageType { get; set; }
    public int Mmsi { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public DateTimeOffset EventTimestamp { get; set; }
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    // Main non position fields
    public double? Sog { get; set; }
    public double? Cog { get; set; }
    public int? Heading { get; set; }
    public NavigationStatus? NavigationStatus { get; set; }
    public double? RateOfTurn { get; set; }
    public bool? PositionAccuracy { get; set; }
    
    // AisStream specific types
    public int? RepeatIndicator { get; set; }
    public bool? Valid { get; set; }
    public int? SpecialManoeuvreIndicator { get; set; }
    public int? Spare { get; set; }
    public bool? Raim { get; set; }
    public int? CommunicationState { get; set; }
}