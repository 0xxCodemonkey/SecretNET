namespace SecretNET.AccessControl;

/// <summary>
/// Class SetViewingKeyRequest (JSON DTO).
/// </summary>
public class SetViewingKeyRequest
{
    /// <summary>
    /// Gets or sets the payload.
    /// </summary>
    /// <value>The payload.</value>
    [JsonProperty("set_viewing_key")]
    public SetViewingKeyRequest_Payload Payload { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SetViewingKeyRequest"/> class.
    /// </summary>
    /// <param name="key">The new viewing key for the message sender.</param>
    /// <param name="padding">An ignored string that can be used to maintain constant message length.</param>
    public SetViewingKeyRequest(string key, string? padding = null)
    {
        Payload = new SetViewingKeyRequest_Payload
        {
            Key = key,
            Padding = padding
        };
    }
}

/// <summary>
/// Class SetViewingKeyRequest_Payload (JSON DTO).
/// </summary>
public class SetViewingKeyRequest_Payload
{
    /// <summary>
    /// The new viewing key for the message sender.
    /// </summary>
    /// <value>The key.</value>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    /// An ignored string that can be used to maintain constant message length.
    /// </summary>
    /// <value>The padding.</value>
    [JsonProperty("padding")]
    public string? Padding { get; set; }
}
