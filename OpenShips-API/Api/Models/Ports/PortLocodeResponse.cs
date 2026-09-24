namespace OpenShipsAPI.Api.Models.Ports;

public class PortLocodeResponse
{
    public int Id { get; set; }
    public string Country { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string? Subregion { get; set; }
    public string? TimeZone { get; set; }
}