using System.Text.Json;
using OpenShipsAPI.Domain.Events;
using OpenShipsAPI.Infrastructure.Queues;
using OpenShipsAPI.Infrastructure.Streams.AisStream.DTOs;
using OpenShipsAPI.Infrastructure.Streams.Common;

namespace OpenShipsAPI.Infrastructure.Streams.AisStream.Handlers;

public sealed class APositionReport(ILogger<APositionReport> logger, AisEventQueue queue) : IAisMessageHandler
{
    public bool CanHandle(JsonElement message, AisSource source)
    {
        if (source != AisSource.AisStream)
            return false;
        
        return message.TryGetProperty(
            "MessageType", out var messageType) && messageType.GetString() == "PositionReport";
    }

    public AisEvent? HandleMessage(JsonElement message, AisSource source)
    {
        var data = message.Deserialize<AisMessage<PositionReportMessage>>();

        if (data == null)
        {
            logger.LogWarning(
                "Failed to deserialize PositionReport message from source {Source}",
                source
            );
            
            return null;
        }

        if (data.Message.PositionReport.Latitude == 0 || data.Message.PositionReport.Longitude == 0)
        {
            logger.LogWarning("PositionReport has empty coordinates!");
            return null;
        }
        
        bool longitudeUnavailable =
            AisUtils.IsUnavailable(
                data.Message.PositionReport.Longitude,
                AisUtils.AisField.Longitude);

        bool latitudeUnavailable =
            AisUtils.IsUnavailable(
                data.Message.PositionReport.Latitude,
                AisUtils.AisField.Latitude);

        if (longitudeUnavailable || latitudeUnavailable)
        {
            return null;
        }
        
        var result = new AisPositionEvent
        {
            Source = source,
            SourceMessageId = null,
            License = AisDataLicense.AisStream,
            Mmsi = data.MetaData.MMSI,
            ReceivedAt = DateTimeOffset.UtcNow,
            MessageType = data.Message.PositionReport.MessageID,
            EventTimestamp =
                TimestampParser
                    .Parse(data.MetaData.time_utc),
            Latitude = data.Message.PositionReport.Latitude,
            Longitude = data.Message.PositionReport.Longitude,
            Sog = AisUtils.IsUnavailable(data.Message.PositionReport.Sog, AisUtils.AisField.Sog) ? null : data.Message.PositionReport.Sog,
            Cog = AisUtils.IsUnavailable(data.Message.PositionReport.Cog, AisUtils.AisField.Cog) ? null : data.Message.PositionReport.Cog,
            Heading = AisUtils.IsUnavailable(data.Message.PositionReport.TrueHeading, AisUtils.AisField.Heading) ? null : data.Message.PositionReport.TrueHeading,
            NavigationStatus =
                Enum.IsDefined(typeof(NavigationStatus), data.Message.PositionReport.NavigationalStatus)
                    ? (NavigationStatus?)data.Message.PositionReport.NavigationalStatus
                    : null,
            RateOfTurn = AisUtils.IsUnavailable(data.Message.PositionReport.RateOfTurn, AisUtils.AisField.Rot) ? null : data.Message.PositionReport.RateOfTurn,
            PositionAccuracy = data.Message.PositionReport.PositionAccuracy,
            RepeatIndicator = data.Message.PositionReport.RepeatIndicator,
            Valid = data.Message.PositionReport.Valid,
            SpecialManoeuvreIndicator = data.Message.PositionReport.SpecialManoeuvreIndicator,
            Spare = data.Message.PositionReport.Spare,
            Raim = data.Message.PositionReport.Raim,
            CommunicationState = data.Message.PositionReport.CommunicationState,
        };
        
        queue.TryEnqueue(result);

        return result;
    }
}