namespace SecretNET.Api;

public enum EncodedPubKeyTypeEnum
{
    tendermint_PubKeySecp256k1,
    tendermint_PubKeyMultisigThreshold
}

public class EncodedPubKey
{
    public EncodedPubKeyTypeEnum Type { get; set; }
    public string AsBase64 { get; set; }
}
