using System.Threading.Channels;
using OpenShipsAPI.Domain.Events;

namespace OpenShipsAPI.Infrastructure.Queues;

public sealed class AisEventQueue
{
    private readonly Channel<AisEvent> _channel =
        Channel.CreateBounded<AisEvent>(
            new BoundedChannelOptions(10_000)
            {
                FullMode = BoundedChannelFullMode.DropWrite,
                SingleReader = true,
                SingleWriter = false
            });

    public bool TryEnqueue(AisEvent aisEvent)
    {
        return _channel.Writer.TryWrite(aisEvent);
    }

    public ChannelReader<AisEvent> Reader => _channel.Reader;
}