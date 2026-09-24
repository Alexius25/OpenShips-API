using System.Threading.Channels;
using OpenShipsAPI.Domain.Destination;

namespace OpenShipsAPI.Infrastructure.Queues;

public sealed class DestinationQueue
{
    private readonly Channel<ShipDestination> _channel =
        Channel.CreateBounded<ShipDestination>(
            new BoundedChannelOptions(10_000)
            {
                FullMode = BoundedChannelFullMode.DropWrite,
                SingleReader = true,
                SingleWriter = false
            });

    public bool TryEnqueue(ShipDestination destination)
    {
        return _channel.Writer.TryWrite(destination);
    }

    public ChannelReader<ShipDestination> Reader =>
        _channel.Reader;
}