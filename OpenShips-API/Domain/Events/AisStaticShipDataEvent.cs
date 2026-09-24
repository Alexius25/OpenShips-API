namespace OpenShipsAPI.Domain.Events;

public class AisStaticShipDataEvent : AisEvent
{
    public string? ShipName { get; set; }
    public int? ShipType  { get; set; }
    public int? ImoNumber { get; set; }
    public string? CallSign { get; set; }
    public string? RawDestination { get; set; }
    public float? Draught  { get; set; }
    public DateTimeOffset? Eta  { get; set; }
    public float? Dim_A { get; set; }
    public float? Dim_B { get; set; }
    public float? Dim_C { get; set; }
    public float? Dim_D { get; set; }
    public float? Length { get; set; }
    public float? Beam { get; set; }
}