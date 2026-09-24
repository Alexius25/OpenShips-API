namespace OpenShipsAPI.Infrastructure.Streams.Common;

public enum NavigationStatus
{
    UnderWay = 0,
    Anchored = 1,
    NotUnderCommand = 2,
    Restricted = 3,
    ConstrainedByDraught = 4,
    Moored = 5,
    Aground = 6,
    EngagedInFishing = 7,
    UnderWaySailing = 8,
    AisSartActive = 14,
}