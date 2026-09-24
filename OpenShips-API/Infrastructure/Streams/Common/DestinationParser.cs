using System.Text.RegularExpressions;
using OpenShipsAPI.Domain.Destination;

namespace OpenShipsAPI.Infrastructure.Streams.Common;

public static class DestinationParser
{
    private static readonly string[] RoundTripSeparators =
    [
        "<->",
        "<>"
    ];
    
    private static readonly string[] DirectedSeparators =
    [
        ">>>",
        ">>",
        "=>",
        ">"
    ];
    
    public static ShipDestination? Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var raw = Normalize(value);

        if (string.IsNullOrWhiteSpace(raw))
            return null;

        // Round trip
        foreach (var separator in RoundTripSeparators)
        {
            if (!raw.Contains(separator, StringComparison.Ordinal))
                continue;

            var parts = raw.Split(
                separator,
                2,
                StringSplitOptions.TrimEntries
            );

            if (parts.Length != 2 ||
                string.IsNullOrWhiteSpace(parts[0]) ||
                string.IsNullOrWhiteSpace(parts[1]))
            {
                return null;
            }

            return new ShipDestination
            {
                Raw = raw,
                RouteType = DestinationType.RoundTrip,
                From = parts[0],
                To = parts[1]
            };
        }

        // Directed route
        foreach (var separator in DirectedSeparators)
        {
            if (!raw.Contains(separator, StringComparison.Ordinal))
                continue;

            var parts = raw.Split(
                separator,
                2,
                StringSplitOptions.TrimEntries
            );

            var from = string.IsNullOrWhiteSpace(parts[0])
                ? null
                : parts[0];

            var to = string.IsNullOrWhiteSpace(parts[1])
                ? null
                : parts[1];

            // Kein sinnvoller Zielteil
            if (to is null)
                return null;

            return new ShipDestination
            {
                Raw = raw,
                RouteType = DestinationType.Directed,
                From = from,
                To = to
            };
        }

        // Keine Routing-Syntax
        return new ShipDestination
        {
            Raw = raw,
            RouteType = DestinationType.Single,
            To = raw
        };
    }

    private static string Normalize(string value)
    {
        return Regex.Replace(
            value.Trim().ToUpperInvariant(),
            @"\s+",
            " "
        );
    }
}