namespace OpenShipsAPI.Infrastructure.Streams.AisStream.DTOs;

public class Eta
{
    public float Minute { get; set; }
    public float Hour { get; set; }
    public float Day { get; set; }
    public float Month { get; set; }

    public static DateTimeOffset? ParseEta(Eta eta, DateTimeOffset timestamp)
    {
        var month = (int)eta.Month;
        var day = (int)eta.Day;
        var hour = (int)eta.Hour;
        var minute = (int)eta.Minute;

        // AIS: ETA not available
        if (month is < 1 or > 12 ||
            day is < 1 or > 31 ||
            hour is < 0 or > 23 ||
            minute is < 0 or > 59)
        {
            return null;
        }

        try
        {
            var etaUtc = new DateTimeOffset(
                timestamp.Year,
                month,
                day,
                hour,
                minute,
                0,
                TimeSpan.Zero);
            
            if (etaUtc < timestamp)
            {
                etaUtc = etaUtc.AddYears(1);
            }

            return etaUtc;
        }
        catch (ArgumentOutOfRangeException)
        {
            // z.B. 31.02.
            return null;
        }
    }
}