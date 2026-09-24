namespace OpenShipsAPI.Infrastructure.Ports;

public class UnLocodeRow
{
    public string? Change { get; set; }
    public string Country { get; set; } = "";
    public string Location { get; set; } = "";
    public string Name { get; set; } = "";
    public string NameWoDiacritics { get; set; } = "";
    public string? Subdivision { get; set; }
    public string? Status { get; set; }
    public string? Function { get; set; }
    public string? Date { get; set; }
    public string? IATA { get; set; }
    public string? Coordinates { get; set; }
    public string? Remarks { get; set; }
    public string? CoordinatesDecimal { get; set; }
    public string? Distance { get; set; }
    public string? Source { get; set; }
}