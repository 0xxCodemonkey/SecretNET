namespace SecretNET.AccessControl;

/// <summary>
/// Class WithPermitRequest (JSON DTO).
/// </summary>
public class WithPermitRequest
{
    /// <summary>
    /// Payload of the request.
    /// </summary>
    [JsonProperty("with_permit")]
    public WithPermitRequest_Payload Payload { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WithPermitRequest" /> class.
    /// </summary>
    /// <param name="permit">The permit.</param>
    /// <param name="queryMsg">The query message.</param>
    public WithPermitRequest(Permit permit, object queryMsg)
    {
        Payload = new WithPermitRequest_Payload
        {
            Permit = permit,
            Query = queryMsg
        };
    }
}

/// <summary>
/// Class WithPermitRequest_Payload (JSON DTO).
/// </summary>
public class WithPermitRequest_Payload
{
    /// <summary>
    /// Gets or sets the permit.
    /// </summary>
    /// <value>The permit.</value>
    [JsonProperty("permit")]
    public object Permit { get; set; }

    /// <summary>
    /// Gets or sets the query.
    /// </summary>
    /// <value>The query.</value>
    [JsonProperty("query")]
    public object Query { get; set; }
}

