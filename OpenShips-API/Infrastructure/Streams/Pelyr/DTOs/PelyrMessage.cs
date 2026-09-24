namespace OpenShipsAPI.Infrastructure.Streams.Pelyr.DTOs;

public class PelyrMessage
{
    public string type { get; set; }
    public string id { get; set; }
    public string license { get; set; }
    public PelyrMessageData data { get; set; }
}

public class PelyrMessageData
{
    public int schema { get; set; }
    public DateTimeOffset ingest_ts { get; set; }
    public DateTimeOffset rx_ts  { get; set; }
    public int mmsi { get; set; }
    public int msg_type { get; set; }
    public double? lat { get; set; }
    public double? lon { get; set; }
    public string? h3_r10 { get; set; }
    public float? sog { get; set; }
    public float? cog { get; set; }
    public int? heading { get; set; }
    public int? nav_status { get; set; }
    public int? rot {get; set;}
    public bool? pos_accuracy { get; set; }
    public string? shipname { get; set; }
    public int? shiptype { get; set; }
    public int? imo { get; set; }
    public string? callsign { get; set; }
    public string? destination  { get; set; }
    public float? draught { get; set; }
    public string? eta { get; set; }
    public int? dim_a { get; set; }
    public int? dim_b { get; set; }
    public int? dim_c { get; set; }
    public int? dim_d { get; set; }
    public int? length { get; set; }
    public int? beam { get; set; }
    public string? dedupe_key { get; set; }
}