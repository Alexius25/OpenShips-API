using OpenShipsAPI.Infrastructure.Streams.Common;

namespace OpenShipsAPI.Worker;

public class MessageCountLogger(
    ILogger<MessageCountLogger> logger)
    : BackgroundService
{
    private readonly Dictionary<(AisSource Source, int MessageType), ulong> _lastCounts = new();
    
    private DateTimeOffset _lastLog = DateTimeOffset.UtcNow;
    
    public bool HasReceivedMessages =>
        AisMessageDispatcher.MessageCounts.Values.Any(x => x > 0);
    
    public IReadOnlyDictionary<(AisSource Source, int MessageType), ulong> GetMessageCounts()
    {
        return AisMessageDispatcher.MessageCounts;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTimeOffset.UtcNow;
            var elapsed = (now - _lastLog).TotalSeconds;

            var lines = AisMessageDispatcher.MessageCounts
                .OrderBy(x => x.Key.Source)
                .ThenBy(x => x.Key.MessageType)
                .Select(x =>
                {
                    var rate = 0.0;

                    if (_lastCounts.TryGetValue(x.Key, out var lastCount))
                    {
                        var delta = x.Value >= lastCount
                            ? x.Value - lastCount
                            : 0;

                        rate = elapsed > 0
                            ? delta / elapsed
                            : 0;
                    }

                    _lastCounts[x.Key] = x.Value;

                    return $"{x.Key.Source}: Message Type {x.Key.MessageType} = {x.Value:N0} ({rate:F1} msg/s)";
                });

            logger.LogInformation(
                "AIS Message Counts ({Time}):\n\n{Counts}",
                now.ToLocalTime(),
                string.Join('\n', lines));

            _lastLog = now;

            await Task.Delay(
                TimeSpan.FromSeconds(10),
                stoppingToken);
        }
    }
}
