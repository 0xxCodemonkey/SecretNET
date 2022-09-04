namespace SecretNET.Tx;

/// <summary>
/// RaAuthenticate defines a message to register an new node.
/// </summary>
public class RaAuthenticate : MsgBase
{
    public override string MsgType { get; } = MsgTypeNames.RaAuthenticate;

    public string Sender { get; set; }

    public byte[] Certificate { get; set; }

    public RaAuthenticate(){}

    public RaAuthenticate(string sender, byte[] certificate)
    {
        Sender = sender;
        Certificate = certificate;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        var msgTransfer = new Secret.Registration.V1Beta1.RaAuthenticate()
        {
            Sender = SecretNetworkClient.AddressToBytes(Sender).GetByteStringFromBase64(),
            Certificate = ByteString.CopyFrom(Certificate)
        };

        return msgTransfer;
    }
    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        var aminoMsg = new AminoMsg("reg/authenticate");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            ra_cert = Convert.ToBase64String(Certificate),
            sender = Sender
        };

        return aminoMsg;
    }
}





