namespace OpenShipsAPI.Api.Models.Vessel;

public class VesselTrackResponse
{
    public int Mmsi { get; set; }
    public int? ShipType { get; init; } 
    public string? ShipName { get; init; }
    public List<VesselTrackPosition> Positions { get; set; } = [];
}