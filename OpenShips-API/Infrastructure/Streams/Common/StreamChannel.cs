using System.Threading.Channels;
using OpenShipsAPI.Domain.Events;

namespace OpenShipsAPI.Infrastructure.Streams.Common;

public class StreamChannel
{
    public Channel<AisEvent> Events { get; }

    public StreamChannel()
    {
        Events = Channel.CreateBounded<AisEvent>(
            new BoundedChannelOptions(10_000)
            {
                FullMode = BoundedChannelFullMode.Wait,

                SingleReader = true,

                SingleWriter = false
            });
    }
}