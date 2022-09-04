namespace SecretNET.AccessControl;

/// <summary>
/// Class Permit (JSON DTO).
/// </summary>
public class Permit
{
    [JsonProperty("params")]
    public Permit_Params Params { get; set; }

    [JsonProperty("signature")]
    public StdSignatureAmino Signature { get; set; }
}

/// <summary>
/// Class Permit_Params (JSON DTO).
/// </summary>
public class Permit_Params
{
    [JsonProperty("permit_name")]
    public string PermitName { get; set; }

    [JsonProperty("allowed_tokens")]
    public string[] AllowedTokens { get; set; }

    [JsonProperty("chain_id")]
    public string ChainId { get; set; }

    [JsonProperty("permissions")]
    public string[] Permissions { get; set; }
}
