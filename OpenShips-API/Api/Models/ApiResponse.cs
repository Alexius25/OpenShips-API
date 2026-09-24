namespace OpenShipsAPI.Api.Models;

public class ApiResponse<T>
{
    public int Version { get; set; }
    public bool Success { get; set; } = true;
    public T? Data { get; set; }
}