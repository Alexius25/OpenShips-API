namespace OpenShipsAPI.Api.Models;

public sealed class BoundingBox
{
    public double MinLon { get; init; }
    public double MaxLon { get; init; }
    public double MinLat { get; init; }
    public double MaxLat { get; init; }

    public bool IsValid =>
        MinLat is >= -90 and <= 90 &&
        MaxLat is >= -90 and <= 90 &&
        MinLat <= MaxLat &&
        MinLon is >= -180 and <= 180 &&
        MaxLon is >= -180 and <= 180;
}