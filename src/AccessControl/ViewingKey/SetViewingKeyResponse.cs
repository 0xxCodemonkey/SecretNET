namespace SecretNET.AccessControl;

/// <summary>
/// Class SetViewingKeyResponse (JSON DTO).
/// </summary>
public class SetViewingKeyResponse
{
    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    /// <value>The result.</value>
    [JsonProperty("set_viewing_key")]
    public SetViewingKeyResponse_Result Result { get; set; }
}

/// <summary>
/// Class SetViewingKeyResponse_Result (JSON DTO).
/// </summary>
public class SetViewingKeyResponse_Result
{
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    /// <value>The status.</value>
    [JsonProperty("status")]
    public string Status { get; set; }
}
