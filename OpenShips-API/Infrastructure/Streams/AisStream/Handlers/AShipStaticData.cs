using System.Text.Json;
using OpenShipsAPI.Domain.Destination;
using OpenShipsAPI.Domain.Events;
using OpenShipsAPI.Infrastructure.Queues;
using OpenShipsAPI.Infrastructure.Streams.AisStream.DTOs;
using OpenShipsAPI.Infrastructure.Streams.Common;

namespace OpenShipsAPI.Infrastructure.Streams.AisStream.Handlers;

public sealed class AShipStaticData(ILogger<AShipStaticData> logger, DestinationQueue destinationQueue, AisEventQueue queue) : IAisMessageHandler
{
    
    public bool CanHandle(JsonElement message, AisSource source)
    {
        if (source != AisSource.AisStream)
            return false;
        
        return message.TryGetProperty(
            "MessageType", out var messageType) && messageType.GetString() == "ShipStaticData";
    }

    public AisEvent? HandleMessage(JsonElement message, AisSource source)
    {
        var data = message.Deserialize<AisMessage<ShipStaticDataMessage>>();

        if (data == null)
        {
            logger.LogWarning(
                "Failed to deserialize PositionReport message from source {Source}",
                source
            );
            
            return null;
        }

        var timestamp = TimestampParser
            .Parse(data.MetaData.time_utc)
            .UtcDateTime;
        
        var result = new AisStaticShipDataEvent()
        {
            Source = source,
            SourceMessageId = null,
            License = AisDataLicense.AisStream,
            Mmsi = data.MetaData.MMSI,
            ReceivedAt = DateTime.UtcNow,
            MessageType = data.Message.ShipStaticData.MessageID,
            EventTimestamp = timestamp,
            ShipName = data.Message.ShipStaticData.Name,
            ShipType = data.Message.ShipStaticData.Type,
            ImoNumber =  data.Message.ShipStaticData.ImoNumber,
            CallSign =  data.Message.ShipStaticData.CallSign,
            RawDestination = data.Message.ShipStaticData.Destination,
            Draught = data.Message.ShipStaticData.MaximumStaticDraught,
            Eta = Eta.ParseEta(data.Message.ShipStaticData.Eta, timestamp),
            Dim_A = data.Message.ShipStaticData.Dimension.A,
            Dim_B = data.Message.ShipStaticData.Dimension.B,
            Dim_C = data.Message.ShipStaticData.Dimension.C,
            Dim_D = data.Message.ShipStaticData.Dimension.D,
            Length = data.Message.ShipStaticData.Dimension.A +  data.Message.ShipStaticData.Dimension.B,
            Beam = data.Message.ShipStaticData.Dimension.C + data.Message.ShipStaticData.Dimension.D
        };
        
        queue.TryEnqueue(result);
        
        var destination = data.Message.ShipStaticData.Destination;
        var parsed = DestinationParser.Parse(destination);
        
        if (parsed != null)
        {
            destinationQueue.TryEnqueue(
                new ShipDestination
                {
                    Mmsi = data.MetaData.MMSI,
                    Raw = parsed.Raw,
                    RouteType = parsed.RouteType,
                    From =  parsed.From,
                    To =  parsed.To,
                    FirstSeen = timestamp,
                    LastSeen = timestamp,
                });
        }

        return result;
    }
}