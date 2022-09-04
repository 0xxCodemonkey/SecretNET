namespace SecretNET.Api;

// https://docs.cosmos.network/main/core/transactions.html#sign-mode-legacy-amino-json
public class StdSignDoc
{
    [JsonProperty("chain_id")]
    public string ChainId { get; set; }

    [JsonProperty("msgs")]
    public AminoMsg[] Msgs { get; set; }

    [JsonProperty("fee")]
    public StdFeeAmino Fee { get; set; }

    [JsonProperty("sequence")]
    public string Sequence { get; set; }

    [JsonProperty("memo")]
    public string? Memo { get; set; }

    [JsonProperty("account_number")]
    public string AccountNumber { get; set; }

}
