using Microsoft.EntityFrameworkCore;
using OpenShipsAPI.Domain.Destination;
using OpenShipsAPI.Infrastructure.Database;
using OpenShipsAPI.Infrastructure.Queues;

namespace OpenShipsAPI.Worker;

public sealed class DestinationWorker : BackgroundService
{
    private const int BatchSize = 500;
    private static readonly TimeSpan BatchInterval = TimeSpan.FromSeconds(2);

    private readonly DestinationQueue _queue;
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly ILogger<DestinationWorker> _logger;
    private readonly DestinationResolver _resolver;

    public DestinationWorker(
        DestinationQueue queue,
        IDbContextFactory<AppDbContext> dbFactory,
        DestinationResolver resolver,
        ILogger<DestinationWorker> logger)
    {
        _queue = queue;
        _dbFactory = dbFactory;
        _resolver = resolver;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var batch = new List<ShipDestination>(BatchSize);

        using var timer = new PeriodicTimer(BatchInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            batch.Clear();

            while (
                batch.Count < BatchSize &&
                _queue.Reader.TryRead(out var item))
            {
                batch.Add(item);
            }

            if (batch.Count == 0)
                continue;

            await ProcessBatchAsync(
                batch,
                stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(
        List<ShipDestination> batch,
        CancellationToken cancellationToken)
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync(
                cancellationToken);

        var grouped = batch
            .GroupBy(x => new
            {
                x.Mmsi,
                x.Raw
            })
            .Select(g =>
            {
                var last = g.MaxBy(x => x.LastSeen)!;

                return new ShipDestination
                {
                    Mmsi = g.Key.Mmsi,
                    Raw = g.Key.Raw,
                    RouteType = last.RouteType,
                    From = last.From,
                    To = last.To,
                    FirstSeen = g.Min(x => x.FirstSeen),
                    LastSeen = g.Max(x => x.LastSeen)
                };
            })
            .ToList();

        var mmsis = grouped
            .Select(x => x.Mmsi)
            .Distinct()
            .ToArray();

        var existing = await db.ShipDestinations
            .Where(x => mmsis.Contains(x.Mmsi))
            .ToListAsync(cancellationToken);

        var lookup = existing.ToDictionary(
            x => (x.Mmsi, x.Raw));

        var resolvedDestinations = await _resolver.ResolveAsync(
            grouped.SelectMany(x => new[]
            {
                x.From,
                x.To
            }),
            cancellationToken);

        var resolvedFromCount = 0;
        var unresolvedFromCount = 0;
        var resolvedToCount = 0;
        var unresolvedToCount = 0;

        foreach (var item in grouped)
        {
            /*
             * -----------------------------------------------------
             * Resolve From
             * -----------------------------------------------------
             */

            if (item.From is not null &&
                resolvedDestinations.TryGetValue(
                    NormalizeForLookup(item.From),
                    out var fromPortId))
            {
                item.FromPortId = fromPortId;
                resolvedFromCount++;
            }
            else if (!string.IsNullOrWhiteSpace(item.From))
            {
                unresolvedFromCount++;

                /*
                _logger.LogWarning(
                    "Unresolved origin: {Origin}",
                    item.From);
                */
            }

            /*
             * -----------------------------------------------------
             * Resolve To
             * -----------------------------------------------------
             */

            if (item.To is not null &&
                resolvedDestinations.TryGetValue(
                    NormalizeForLookup(item.To),
                    out var toPortId))
            {
                item.ToPortId = toPortId;
                resolvedToCount++;
            }
            else if (!string.IsNullOrWhiteSpace(item.To))
            {
                unresolvedToCount++;

                /*
                _logger.LogWarning(
                    "Unresolved destination: {Destination}",
                    item.To);
                */
            }

            /*
             * -----------------------------------------------------
             * Update existing destination
             * -----------------------------------------------------
             */

            if (lookup.TryGetValue(
                    (item.Mmsi, item.Raw),
                    out var current))
            {
                if (item.FirstSeen < current.FirstSeen)
                    current.FirstSeen = item.FirstSeen;

                if (item.LastSeen > current.LastSeen)
                    current.LastSeen = item.LastSeen;

                // Do not overwrite an existing successful
                // resolution with null.
                if (item.FromPortId.HasValue)
                    current.FromPortId = item.FromPortId;

                if (item.ToPortId.HasValue)
                    current.ToPortId = item.ToPortId;
            }
            else
            {
                db.ShipDestinations.Add(item);

                lookup.Add(
                    (item.Mmsi, item.Raw),
                    item);
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        /*
        _logger.LogInformation(
            "Processed {Count} unique destinations: " +
            "From {ResolvedFrom} resolved, {UnresolvedFrom} unresolved; " +
            "To {ResolvedTo} resolved, {UnresolvedTo} unresolved",
            grouped.Count,
            resolvedFromCount,
            unresolvedFromCount,
            resolvedToCount,
            unresolvedToCount);
            */
    }

    private static string NormalizeForLookup(string value)
    {
        var normalized = value
            .Trim()
            .ToLowerInvariant();

        var compact = new string(
            normalized
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());

        if (IsUnLocode(compact))
            return compact;

        return normalized;
    }

    private static bool IsUnLocode(string value)
    {
        return value.Length == 5 &&
               value[..2].All(char.IsLetter) &&
               value[2..].All(char.IsLetterOrDigit);
    }
}