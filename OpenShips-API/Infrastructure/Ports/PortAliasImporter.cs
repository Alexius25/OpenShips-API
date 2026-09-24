using CsvHelper;
using Microsoft.EntityFrameworkCore;
using OpenShipsAPI.Infrastructure.Database;
using OpenShipsAPI.Infrastructure.Database.Models;
using System.Globalization;
using CsvHelper.Configuration;

namespace OpenShipsAPI.Infrastructure.Ports;

public class PortAliasImporter
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly HttpClient _httpClient;

    private const int BatchSize = 2_000;

    private const string SourceUrl =
        "https://github.com/cristan/improved-un-locodes/raw/refs/heads/main/data/aliases-improved.csv";

    public PortAliasImporter(
        IDbContextFactory<AppDbContext> dbFactory,
        HttpClient httpClient)
    {
        _dbFactory = dbFactory;
        _httpClient = httpClient;
    }

    public async Task ImportAsync(
        CancellationToken cancellationToken = default)
    {
        var batch = new List<UnLocodeAliasRow>(BatchSize);

        var totalRecords = 0;
        var aliasRecords = 0;
        var skippedRecords = 0;
        var batchNumber = 0;

        Console.WriteLine("Loading Alias-Data from GitHub...");
        Console.WriteLine(SourceUrl);
        Console.WriteLine();

        await using var stream = await _httpClient.GetStreamAsync(
            SourceUrl,
            cancellationToken);

        using var reader = new StreamReader(stream);

        using var csv = new CsvReader(
            reader,
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null
            });

        foreach (var record in csv.GetRecords<UnLocodeAliasRow>())
        {
            cancellationToken.ThrowIfCancellationRequested();

            totalRecords++;

            if (string.IsNullOrWhiteSpace(record.Unlocode) ||
                string.IsNullOrWhiteSpace(record.Alias))
            {
                skippedRecords++;
                continue;
            }

            batch.Add(record);

            if (batch.Count < BatchSize)
                continue;

            batchNumber++;

            var result = await ImportBatchAsync(
                batch,
                cancellationToken);

            aliasRecords += result.Imported;
            skippedRecords += result.Skipped;

            Console.WriteLine(
                $"Batch {batchNumber}: " +
                $"Saved {result.Imported:N0} Aliases | " +
                $"{result.Skipped:N0} skipped | " +
                $"CSV: {totalRecords:N0}");

            batch.Clear();
        }
        
        if (batch.Count > 0)
        {
            batchNumber++;

            var result = await ImportBatchAsync(
                batch,
                cancellationToken);

            aliasRecords += result.Imported;
            skippedRecords += result.Skipped;

            Console.WriteLine(
                $"Batch {batchNumber}: " +
                $"Saved {result.Imported:N0} Aliases | " +
                $"{result.Skipped:N0} skipped | " +
                $"CSV: {totalRecords:N0}");

            batch.Clear();
        }

        Console.WriteLine();
        Console.WriteLine("Alias-Import completed.");
        Console.WriteLine($"CSV-Entries: {totalRecords:N0}");
        Console.WriteLine($"Aliases:     {aliasRecords:N0}");
        Console.WriteLine($"Skipped:     {skippedRecords:N0}");
        Console.WriteLine($"Batches:     {batchNumber:N0}");
    }

    private async Task<(int Imported, int Skipped)> ImportBatchAsync(
        List<UnLocodeAliasRow> batch,
        CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(
            cancellationToken);
        
        var keys = batch
            .Select(x => ParseUnlocode(x.Unlocode))
            .Where(x => x is not null)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

        if (keys.Count == 0)
            return (0, batch.Count);

        var countries = keys
            .Select(x => x.Country)
            .Distinct()
            .ToList();

        var locations = keys
            .Select(x => x.Location)
            .Distinct()
            .ToList();
        
        var ports = await db.Ports
            .Where(x =>
                countries.Contains(x.Country) &&
                locations.Contains(x.Location))
            .ToListAsync(cancellationToken);

        var portsByUnlocode = ports.ToDictionary(
            x => (x.Country, x.Location));
        
        var portIds = ports
            .Select(x => x.Id)
            .ToList();

        var existingAliases = await db.Set<PortAliasDb>()
            .Where(x => portIds.Contains(x.PortId))
            .ToListAsync(cancellationToken);

        var existingAliasesByKey = existingAliases
            .Select(x => (
                x.PortId,
                Alias: x.Alias))
            .ToHashSet();

        var imported = 0;
        var skipped = 0;

        foreach (var row in batch)
        {
            var key = ParseUnlocode(row.Unlocode);

            if (key is null)
            {
                skipped++;
                continue;
            }

            if (!portsByUnlocode.TryGetValue(
                    key.Value,
                    out var port))
            {
                skipped++;
                continue;
            }

            var alias = row.Alias.Trim();

            if (alias.Length == 0)
            {
                skipped++;
                continue;
            }

            var aliasKey = (
                port.Id,
                Alias: alias);

            if (existingAliasesByKey.Contains(aliasKey))
            {
                skipped++;
                continue;
            }

            var aliasEntity = new PortAliasDb
            {
                PortId = port.Id,
                Alias = alias
            };

            db.Set<PortAliasDb>().Add(aliasEntity);
            
            existingAliasesByKey.Add(aliasKey);

            imported++;
        }

        await db.SaveChangesAsync(cancellationToken);

        return (imported, skipped);
    }

    private static (string Country, string Location)? ParseUnlocode(
        string? unlocode)
    {
        if (string.IsNullOrWhiteSpace(unlocode))
            return null;

        var value = unlocode.Trim().ToUpperInvariant();

        /*
         * UN/LOCODE:
         *
         * 2 characters Country
         * 3 characters Location
         */
        if (value.Length != 5)
            return null;

        return (
            Country: value[..2],
            Location: value[2..]);
    }
}