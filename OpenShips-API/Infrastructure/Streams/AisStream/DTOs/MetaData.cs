namespace OpenShipsAPI.Infrastructure.Streams.AisStream.DTOs;

public class MetaData
{
    public int MMSI { get; set; }
    public string ShipName { get; set; } = string.Empty;
    public double latitude { get; set; }
    public double longitude { get; set; }
    public string time_utc { get; set; }
}