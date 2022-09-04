namespace SecretNET.Tx;

/// <summary>
/// RaAuthenticate defines a message to register an new node.
/// </summary>
public class MsgUnjail : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgUnjail;

    public string ValidatorAddress { get; set; }

    public MsgUnjail(){}

    public MsgUnjail(string validatorAddress)
    {
        ValidatorAddress = validatorAddress;
    }

    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        var msgUnjail = new Cosmos.Slashing.V1Beta1.MsgUnjail()
        {
            ValidatorAddr = ValidatorAddress,
        };

        return msgUnjail;
    }
    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgUnjail");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            address = ValidatorAddress
        };

        return aminoMsg;
    }
}





