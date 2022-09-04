namespace SecretNET.AccessControl;

/// <summary>
/// Class CreateViewingKeyRequest (JSON DTO).
/// </summary>
public class CreateViewingKeyRequest
{
    /// <summary>
    /// Gets or sets the payload.
    /// </summary>
    /// <value>The payload.</value>
    [JsonProperty("create_viewing_key")]
    public CreateViewingKeyOptions_Payload Payload { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateViewingKeyRequest"/> class.
    /// </summary>
    /// <param name="entropy">String used as part of the entropy supplied to the rng that generates the random viewing key.</param>
    /// <param name="padding">An ignored string that can be used to maintain constant message length.</param>
    public CreateViewingKeyRequest(string entropy, string? padding = null)
    {
        Payload = new CreateViewingKeyOptions_Payload
        {
            Entropy = entropy,
            Padding = padding
        };
    }
}

/// <summary>
/// Class CreateViewingKeyOptions_Payload (JSON DTO).
/// </summary>
public class CreateViewingKeyOptions_Payload
{
    /// <summary>
    /// String used as part of the entropy supplied to the rng that generates the random viewing key (not optional).
    /// </summary>
    /// <value>The entropy.</value>
    [JsonProperty("entropy")]
    public string Entropy { get; set; }

    /// <summary>
    /// An ignored string that can be used to maintain constant message length.
    /// </summary>
    /// <value>The padding.</value>
    [JsonProperty("padding")]
    public string? Padding { get; set; }
}
