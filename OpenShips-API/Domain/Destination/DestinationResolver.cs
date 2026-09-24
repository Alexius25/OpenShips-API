using Microsoft.EntityFrameworkCore;
using OpenShipsAPI.Infrastructure.Database;

namespace OpenShipsAPI.Domain.Destination;

public sealed class DestinationResolver
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public DestinationResolver(
        IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<Dictionary<string, int>> ResolveAsync(
        IEnumerable<string?> destinations,
        CancellationToken cancellationToken)
    {
        var values = destinations
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(Normalize)
            .Distinct()
            .ToArray();

        if (values.Length == 0)
            return [];

        var unlocodes = values
            .Select(TryParseUnLocode)
            .Where(x => x is not null)
            .Select(x => x!.Value)
            .Distinct()
            .ToArray();

        await using var db =
            await _dbFactory.CreateDbContextAsync(
                cancellationToken);

        var result = new Dictionary<string, int>();

        /*
         * ---------------------------------------------------------
         * 1. Resolve UN/LOCODEs
         * ---------------------------------------------------------
         */

        if (unlocodes.Length > 0)
        {
            var unlocodeKeys = unlocodes
                .Select(x => x.Compact)
                .ToArray();

            var portsByUnLocode = await db.Ports
                .AsNoTracking()
                .Where(x =>
                    unlocodeKeys.Contains(
                        x.Country.ToLower() +
                        x.Location.ToLower()))
                .Select(x => new
                {
                    x.Id,
                    x.Country,
                    x.Location
                })
                .ToListAsync(cancellationToken);

            foreach (var port in portsByUnLocode)
            {
                var key =
                    port.Country.ToLowerInvariant() +
                    port.Location.ToLowerInvariant();

                result[key] = port.Id;
            }
        }

        /*
         * ---------------------------------------------------------
         * 2. Resolve names and aliases in one batch
         * ---------------------------------------------------------
         */

        var textValues = values
            .Where(x => !IsUnLocode(x))
            .ToArray();

        if (textValues.Length == 0)
            return result;

        var ports = await db.Ports
            .AsNoTracking()
            .Where(x =>
                textValues.Contains(x.Name.ToLower()) ||
                (x.NameWoDiacritics != null &&
                 textValues.Contains(x.NameWoDiacritics.ToLower())) ||
                x.Aliases.Any(a =>
                    textValues.Contains(a.Alias.ToLower())))
            .Select(x => new
            {
                x.Id,
                x.Country,
                x.Location,
                x.Name,
                x.NameWoDiacritics,
                Aliases = x.Aliases
                    .Select(a => a.Alias)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        foreach (var port in ports)
        {
            Add(result, port.Name, port.Id);
            Add(result, port.NameWoDiacritics, port.Id);

            foreach (var alias in port.Aliases)
                Add(result, alias, port.Id);
        }

        return result;
    }

    private static void Add(
        Dictionary<string, int> result,
        string? value,
        int portId)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        result.TryAdd(
            Normalize(value),
            portId);
    }

    private static bool IsUnLocode(string value)
    {
        return TryParseUnLocode(value) is not null;
    }

    private static UnLocode? TryParseUnLocode(string value)
    {
        var compact = new string(
            value
                .Trim()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());

        if (compact.Length != 5)
            return null;

        if (!compact[..2].All(char.IsLetter))
            return null;

        if (!compact[2..].All(char.IsLetterOrDigit))
            return null;

        return new UnLocode(
            Country: compact[..2].ToLowerInvariant(),
            Location: compact[2..].ToLowerInvariant(),
            Compact: compact.ToLowerInvariant());
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .ToLowerInvariant();
    }

    private readonly record struct UnLocode(
        string Country,
        string Location,
        string Compact);
}