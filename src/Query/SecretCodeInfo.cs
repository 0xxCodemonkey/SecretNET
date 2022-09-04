namespace SecretNET.Query;

public class SecretCodeInfo
{
    /// <summary>
    /// 
    /// </summary>
    public ulong CodeId { get; set; }

    /// <summary>
    /// The address of the creator
    /// </summary>
    public string CreatorAddress { get; set; }

    /// <summary>
    /// The SHA256 hash value of the contract's WASM bytecode, represented as case-insensitive 64
    /// character hex string.
    /// This is used to make sure only the contract that's being invoked can decrypt the query data.
    /// </summary>
    public string CodeHash { get; set; }

    /// <summary>
    /// Source is a valid absolute HTTPS URI to the contract's source code, optional
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Builder is a valid docker image name with tag, optional
    /// AccessConfig instantiate_config = 5 [(gogoproto.nullable) = false];
    /// </summary>
    public string Builder { get; set; }

}
