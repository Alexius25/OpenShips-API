using System.Text.Json;

namespace OpenShipsAPI.Infrastructure.Streams.Common;

public static class AisUtils
{
    public static bool TryGetMessageType(JsonElement message, ILogger logger, AisSource source, out int messageType)
    {
        messageType = 0;

        return source switch
        {
            AisSource.Pelyr => TryGetPelyrMessageType(message, logger, out messageType),
            AisSource.AisStream => TryGetAisStreamMessageType(message, out messageType),
            _ => false
        };
    }

    private static bool TryGetPelyrMessageType(JsonElement message, ILogger logger, out int messageType)
    {
        messageType = 0;

        if (!message.TryGetProperty("type", out var typeElement))
        {
            logger.LogWarning(
                "Pelyr message has no 'type' property: {Message}",
                message.GetRawText());

            return false;
        }

        var type = typeElement.GetString();

        if (type is "subscribed" or "heartbeat" or "welcome")
            return false;

        if (!message.TryGetProperty("data", out var data))
            return false;

        if (!data.TryGetProperty("msg_type", out var msgTypeElement))
            return false;

        messageType = msgTypeElement.GetInt32();

        return true;
    }

    private static bool TryGetAisStreamMessageType(JsonElement message, out int messageType)
    {
        messageType = 0;

        if (message.ValueKind != JsonValueKind.Object ||
            !message.TryGetProperty("Message", out var messageElement) ||
            messageElement.ValueKind != JsonValueKind.Object)
            return false;

        foreach (var property in messageElement.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.Object)
                continue;

            if (!property.Value.TryGetProperty("MessageID", out var messageId))
                continue;

            if (!messageId.TryGetInt32(out messageType))
                return false;

            return true;
        }

        return false;
    }

    public static bool IsUnavailable(double value, AisField field)
    {
        return field switch
        {
            AisField.Latitude  => value == 91.0,
            AisField.Longitude => value == 181.0,
            AisField.Sog       => value == 102.3,
            AisField.Cog       => value == 360.0,
            AisField.Heading   => value == 511.0,
            AisField.Rot       => value == -128.0,
            _ => false
        };
    }

    public enum AisField
    {
        Latitude,
        Longitude,
        Sog,
        Cog,
        Heading,
        Rot
    }
}