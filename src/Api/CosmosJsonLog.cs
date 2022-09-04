namespace SecretNET.Api;

public class CosmosJsonLog
{
    [JsonProperty("msg_index")]
    public int? MsgIndex { get; set; }

    [JsonProperty("events")]
    public List<CosmosJsonLogEvent> Events { get; set; } = new List<CosmosJsonLogEvent>();

}

public class CosmosJsonLogEvent
{
    [JsonProperty("type")]
    public string MessageType { get; set; }

    [JsonProperty("attributes")]
    public List<CosmosJsonLogKeyValue> Attributes { get; set; } = new List<CosmosJsonLogKeyValue>();
}

public class CosmosJsonLogKeyValue
{
    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }
}
