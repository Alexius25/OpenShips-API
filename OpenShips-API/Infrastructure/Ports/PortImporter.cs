using CsvHelper;
using Microsoft.EntityFrameworkCore;
using OpenShipsAPI.Infrastructure.Database;
using OpenShipsAPI.Infrastructure.Database.Models;
using System.Globalization;
using OpenShipsAPI.Utility;

namespace OpenShipsAPI.Infrastructure.Ports;

public class PortImporter
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly HttpClient _httpClient;

    private const int BatchSize = 2_000;

    private const string SourceUrl =
        "https://raw.githubusercontent.com/cristan/improved-un-locodes/refs/heads/main/data/code-list-improved.csv";

    public PortImporter(
        IDbContextFactory<AppDbContext> dbFactory,
        HttpClient httpClient)
    {
        _dbFactory = dbFactory;
        _httpClient = httpClient;
    }

    public async Task ImportAsync(
        CancellationToken cancellationToken = default)
    {
        var batch = new List<PortDb>(BatchSize);

        var totalRecords = 0;
        var portRecords = 0;
        var batchNumber = 0;

        Console.WriteLine("Downloading UN/LOCODE data from GitHub...");
        Console.WriteLine(SourceUrl);
        Console.WriteLine();

        await using var stream = await _httpClient.GetStreamAsync(
            SourceUrl,
            cancellationToken);

        using var reader = new StreamReader(stream);

        using var csv = new CsvReader(
            reader,
            CultureInfo.InvariantCulture);

        foreach (var record in csv.GetRecords<UnLocodeRow>())
        {
            cancellationToken.ThrowIfCancellationRequested();

            totalRecords++;

            // Only import records whose function starts with "1" (port).
            if (record.Function is not { Length: > 0 } function ||
                function[0] != '1')
            {
                continue;
            }

            var port = MapToPort(record);

            batch.Add(port);
            portRecords++;

            if (batch.Count < BatchSize)
                continue;

            batchNumber++;

            var count = batch.Count;

            await UpsertBatchAsync(
                batch,
                cancellationToken);

            Console.WriteLine(
                $"Batch {batchNumber}: " +
                $"{count:N0} ports processed | " +
                $"CSV records: {totalRecords:N0} | " +
                $"ports total: {portRecords:N0}");

            batch.Clear();
        }

        // Process the remaining records.
        if (batch.Count > 0)
        {
            batchNumber++;

            var count = batch.Count;

            await UpsertBatchAsync(
                batch,
                cancellationToken);

            Console.WriteLine(
                $"Batch {batchNumber}: " +
                $"{count:N0} ports processed | " +
                $"CSV records: {totalRecords:N0} | " +
                $"ports total: {portRecords:N0}");

            batch.Clear();
        }

        Console.WriteLine();
        Console.WriteLine("Port import completed.");
        Console.WriteLine($"CSV records: {totalRecords:N0}");
        Console.WriteLine($"Ports:       {portRecords:N0}");
        Console.WriteLine($"Batches:     {batchNumber:N0}");
    }

    private async Task UpsertBatchAsync(
        List<PortDb> batch,
        CancellationToken cancellationToken)
    {
        if (batch.Count == 0)
            return;

        await using var db =
            await _dbFactory.CreateDbContextAsync(cancellationToken);

        /*
         * Build the actual UN/LOCODE keys contained in this batch.
         *
         * Using separate Country/Location IN clauses would also match
         * unrelated combinations. For example:
         *
         *   DE + HAM
         *   FR + CDG
         *
         * could also match:
         *
         *   DE + CDG
         *   FR + HAM
         *
         * The key lookup below avoids that problem.
         */
        var keys = batch
            .Select(x => (x.Country, x.Location))
            .Distinct()
            .ToList();

        var countries = keys
            .Select(x => x.Country)
            .Distinct()
            .ToList();

        var locations = keys
            .Select(x => x.Location)
            .Distinct()
            .ToList();

        /*
         * EF Core cannot translate a tuple collection Contains()
         * consistently across all database providers, so use the
         * individual columns for the initial database lookup and
         * filter the exact combinations in memory.
         *
         * The Country + Location unique index keeps this lookup fast.
         */
        var existingPorts = await db.Ports
            .AsNoTracking()
            .Where(x =>
                countries.Contains(x.Country) &&
                locations.Contains(x.Location))
            .ToListAsync(cancellationToken);

        var existingByKey = existingPorts
            .Where(x => keys.Contains((x.Country, x.Location)))
            .ToDictionary(
                x => (x.Country, x.Location));

        /*
         * Attach existing entities only when they actually need
         * to be updated. New entities are added normally.
         */
        foreach (var port in batch)
        {
            var key = (port.Country, port.Location);

            if (port.Latitude is double latitude &&
                port.Longitude is double longitude)
            {
                var timezone = TimeZoneUtils.GetTimeZoneString(latitude, longitude);
                port.TimeZone = timezone;
            }
            
            if (existingByKey.TryGetValue(
                    key,
                    out var existingPort))
            {
                port.Id = existingPort.Id;

                db.Ports.Update(port);
            }
            else
            {
                db.Ports.Add(port);
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        /*
         * Clear tracked entities after every batch so memory usage
         * does not continuously grow during large imports.
         */
        db.ChangeTracker.Clear();
    }

    private static PortDb MapToPort(
        UnLocodeRow row)
    {
        var (latitude, longitude) =
            ParseCoordinates(row.CoordinatesDecimal);

        return new PortDb
        {
            Country = row.Country,
            Location = row.Location,

            Name = row.Name,
            NameWoDiacritics = row.NameWoDiacritics,

            Subregion = row.Subdivision,
            Function = row.Function!,

            Latitude = latitude,
            Longitude = longitude,

            Remarks = row.Remarks,
            Source = row.Source
        };
    }

    private static (
        double? Latitude,
        double? Longitude)
        ParseCoordinates(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return (null, null);

        var parts = value.Split(
            ',',
            StringSplitOptions.TrimEntries);

        if (parts.Length != 2)
            return (null, null);

        var latitudeOk = double.TryParse(
            parts[0],
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out var latitude);

        var longitudeOk = double.TryParse(
            parts[1],
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out var longitude);

        return (
            latitudeOk ? latitude : null,
            longitudeOk ? longitude : null);
    }
}