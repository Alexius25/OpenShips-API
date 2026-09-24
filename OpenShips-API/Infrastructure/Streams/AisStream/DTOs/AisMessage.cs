namespace OpenShipsAPI.Infrastructure.Streams.AisStream.DTOs;

public class AisMessage<T>
{
    public string MessageType { get; set; } = string.Empty;
    public MetaData MetaData { get; set; } = new();
    public T Message { get; set; } = default!;
}
