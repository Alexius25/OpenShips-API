using System.Text.Json;
using OpenShipsAPI.Domain.Events;
using OpenShipsAPI.Infrastructure.Queues;
using OpenShipsAPI.Infrastructure.Streams.Common;
using OpenShipsAPI.Infrastructure.Streams.Pelyr.DTOs;

namespace OpenShipsAPI.Infrastructure.Streams.Pelyr.Handlers;

public sealed class PositionHandler(ILogger<PositionHandler> logger, AisEventQueue queue) : IAisMessageHandler
{
    public bool CanHandle(JsonElement message, AisSource source)
    {
        if (source != AisSource.Pelyr)
            return false;
        
        bool hasMsgType = AisUtils.TryGetMessageType(message, logger, AisSource.Pelyr, out var messageType);

        if (!hasMsgType)
            return false;
        
        return messageType is 1 or 2 or 3;
    }
    
    public AisEvent? HandleMessage(JsonElement message, AisSource source)
    {
        var data = message.Deserialize<PelyrMessage>();

        if (data == null)
        {
            logger.LogWarning(
                "Failed to deserialize PositionReport message from source {Source}",
                source
            );
            
            return null;
        }

        if (data.data.lat is null || data.data.lon is null)
        {
            logger.LogDebug(
                "PositionReport message from source {Source} has null latitude or longitude",
                source
            );
            
            return null;
        }
        
        var result = new AisPositionEvent
        {
            Source = source,
            SourceMessageId = data.id,
            License = Enum.TryParse(data.license, out AisDataLicense license) ? license : AisDataLicense.Pelyr,
            MessageType = data.data.msg_type,
            Mmsi = data.data.mmsi,
            ReceivedAt = data.data.ingest_ts,
            EventTimestamp = data.data.rx_ts,
            Latitude = data.data.lat.Value,
            Longitude = data.data.lon.Value,
            Sog = data.data.sog,
            Cog = data.data.cog,
            Heading = data.data.heading,
            NavigationStatus =
                Enum.IsDefined(typeof(NavigationStatus), data.data.nav_status)
                    ? (NavigationStatus?)data.data.nav_status
                    : null,
            RateOfTurn = data.data.rot,
            PositionAccuracy = data.data.pos_accuracy
        };
        
        queue.TryEnqueue(result);
        
        return result;
    }
}