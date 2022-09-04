namespace SecretNET.Api;


/// <summary>
/// Class AminoMsg (JSON DTO).
/// </summary>
public class AminoMsg
{
    /// <summary>
    /// Message type.
    /// </summary>
    /// <value>The type.</value>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// The value.
    /// </summary>
    /// <value>The value.</value>
    [JsonProperty("value")]
    public object Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AminoMsg"/> class.
    /// </summary>
    /// <param name="typeUrl">The type URL.</param>
    public AminoMsg(string typeUrl)
    {
        Type = typeUrl;
    }
}

