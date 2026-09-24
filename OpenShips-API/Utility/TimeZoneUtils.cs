using GeoTimeZone;

namespace OpenShipsAPI.Utility;

public static class TimeZoneUtils
{
    public static string GetTimeZoneString(double latitude, double longitude)
    {
        return TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
    }

    public static TimeZoneInfo GetTimeZone(double latitude, double longitude)
    {
        string id = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
        return TimeZoneInfo.FindSystemTimeZoneById(id);
    }
}