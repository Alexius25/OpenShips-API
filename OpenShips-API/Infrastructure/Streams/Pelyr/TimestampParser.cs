using System.Globalization;

namespace OpenShipsAPI.Infrastructure.Streams.Pelyr;

public class TimestampParser
{
    /// <summary>
    /// Parses an Pelyr Message ETA (Estimated Time of Arrival) from a string value.
    /// </summary>
    /// <param name="value">The ETA value as a string.</param>
    /// <param name="timestamp">The timestamp to use as a reference.</param>
    /// <returns>The parsed ETA as a DateTimeOffset.</returns>
    public static DateTimeOffset ParseETAPelyr(string value, DateTimeOffset timestamp)
    {
        var eta = DateTimeOffset.ParseExact(
            $"{timestamp.Year}-{value}+00:00",
            "yyyy-MM-ddTHH:mmzzz",
            CultureInfo.InvariantCulture
        );

        if (eta < timestamp)
        {
            eta = eta.AddYears(1);
        }

        return eta;
    }
}