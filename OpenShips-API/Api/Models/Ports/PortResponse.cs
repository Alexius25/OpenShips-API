namespace OpenShipsAPI.Api.Models.Ports;

public class PortResponse
{
    public int Id { get; set; }
    public string Country { get; set; } = null!;
    public string Location { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    public string? NameWoDiacritics { get; set; }
    
    public string? Subregion { get; set; }
    public string Function { get; set; } = null!;
    
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    public string? Remarks { get; set; }
    public string? Source { get; set; }
    
    public string? TimeZone { get; set; }

    public List<string> Aliases { get; set; } = [];
}