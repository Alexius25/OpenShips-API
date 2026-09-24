namespace OpenShipsAPI.Infrastructure.Streams.AisStream.DTOs;

public sealed class ShipStaticDataMessage
{
    public ShipStaticData ShipStaticData { get; set; } = new();
}


public class ShipStaticData
{
    public int MessageID  { get; set; }
    public int RepeatIndicator { get; set; }
    public int UserID  { get; set; }
    public bool Valid { get; set; }
    public int AisVersion { get; set; }
    public int ImoNumber { get; set; }
    public string CallSign { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; }
    public Dimension Dimension { get; set; } = new();
    public int FixType { get; set; }
    public Eta Eta { get; set; } = new();
    public float MaximumStaticDraught  { get; set; }
    public string Destination  { get; set; } = string.Empty;
    public bool Dte { get; set; }
    public bool Spare  { get; set; }
}