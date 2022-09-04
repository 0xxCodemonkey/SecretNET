namespace SecretNET.Api;

public class StdSignature
{
    public string Signature { get; set; }
    public EncodedPubKey PubKey { get; set; }
}

public class StdSignatureAmino
{
    [JsonProperty("signature")]
    public string Signature { get; set; }

    [JsonProperty("pub_key")]
    public Pubkey PubKey { get; set; }

    public StdSignatureAmino(string signature, byte[] pubKey, string type = "tendermint/PubKeySecp256k1")
    {
        Signature = signature;
        PubKey = new Pubkey(pubKey, type);
    }
}

public class Pubkey
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("value")]
    public string AsBase64 { get; set; }

    public Pubkey(byte[] pubKey, string type = "tendermint/PubKeySecp256k1")
    {
        if (pubKey.Length != 33 || (pubKey[0] != 0x02 && pubKey[0] != 0x03))
        {
            throw new Exception("Public key must be compressed secp256k1, i.e. 33 bytes starting with 0x02 or 0x03");
        }

        Type = type;
        AsBase64 = Convert.ToBase64String(pubKey);
    }
}


