namespace SecretNET.Tx;

public class Coin
{
    /// <summary>
    /// The denomination of the native Coin (uscrt for SCRT).
    /// </summary>
    [JsonProperty("denom")]
    public string Denom { get; set; } = "uscrt";

    /// <summary>
    /// The amount of the native Coin.
    /// </summary>
    [JsonProperty("amount")]
    public string Amount { get; set; }
}
