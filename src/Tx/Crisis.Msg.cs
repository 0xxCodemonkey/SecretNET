namespace SecretNET.Tx;

public class MsgVerifyInvariant : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgVerifyInvariant;

    public string Sender { get; set; }

    public string InvariantModuleName { get; set; }

    public string InvariantRoute { get; set; }


    public MsgVerifyInvariant() { }

    public MsgVerifyInvariant(string sender, string invariantModuleName, string invariantRoute)
    {
        Sender = sender;
        InvariantModuleName = invariantModuleName;
        InvariantRoute = invariantRoute;
    }


    public override async Task<IMessage> ToProto(SecretEncryptionUtils utils)
    {
        var msgVerifyInvariant = new Cosmos.Crisis.V1Beta1.MsgVerifyInvariant()
        {
            Sender = Sender,
            InvariantModuleName = InvariantModuleName,
            InvariantRoute = InvariantRoute,
        };

        return msgVerifyInvariant;
    }

    public override async Task<AminoMsg> ToAmino(SecretEncryptionUtils utils)
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgVerifyInvariant");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            invariant_module_name = InvariantModuleName,
            invariant_route = InvariantRoute,
            sender = Sender
        };

        return aminoMsg;
    }
}
