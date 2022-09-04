namespace SecretNET.AccessControl;

/// <summary>
/// Class CreateViewingKeyResponse (JSON DTO).
/// </summary>
public class CreateViewingKeyResponse
{
    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    /// <value>The result.</value>
    [JsonProperty("create_viewing_key")]
    public CreateViewingKeyResponse_Result Result { get; set; }
}

/// <summary>
/// Class CreateViewingKeyResponse_Result (JSON DTO).
/// </summary>
public class CreateViewingKeyResponse_Result
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    /// <value>The key.</value>
    [JsonProperty("key")]
    public string Key { get; set; }
}
