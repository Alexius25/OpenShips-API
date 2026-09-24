using System.Globalization;

namespace OpenShipsAPI.Infrastructure.Streams.AisStream;

public static class TimestampParser
{
    public static DateTimeOffset Parse(string value)
    {
        var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 4)
            throw new FormatException(
                $"Invalid AIS timestamp: '{value}'");

        var date = parts[0];
        var time = parts[1];
        var offset = parts[2];

        var dotIndex = time.IndexOf('.');

        if (dotIndex >= 0)
        {
            var fraction = time[(dotIndex + 1)..];

            if (fraction.Length > 7)
                fraction = fraction[..7];

            time =
                time[..dotIndex]
                + "."
                + fraction;
        }

        // +0000 -> +00:00
        if (offset.Length == 5)
        {
            offset =
                offset[..3]
                + ":"
                + offset[3..];
        }

        var normalized =
            $"{date} {time} {offset}";

        return DateTimeOffset.ParseExact(
            normalized,
            "yyyy-MM-dd HH:mm:ss.FFFFFFF zzz",
            CultureInfo.InvariantCulture);
    }
}