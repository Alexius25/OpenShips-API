namespace OpenShipsAPI.Infrastructure.Database.Models;

public class PortAliasDb
{
    public int Id { get; set; }
    public int PortId { get; set; }
    public string Alias { get; set; } = null!;
    public PortDb Port { get; set; } = null!;
}