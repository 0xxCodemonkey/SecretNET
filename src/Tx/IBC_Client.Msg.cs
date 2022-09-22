namespace SecretNET.Tx;

/// <summary>
/// MsgUpdateClient defines an sdk.Msg to update a IBC client state using the given header.
/// </summary>
public class MsgUpgradeClient : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgUpgradeClient;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgUpgradeClient ToAmino is not implemented.");
    }

    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgUpgradeClient ToProto is not implemented.");
    }
}

/// <summary>
/// MsgSubmitMisbehaviour defines an sdk.Msg type that submits Evidence for light client misbehaviour.
/// </summary>
public class MsgSubmitMisbehaviour : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgSubmitMisbehaviour;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgSubmitMisbehaviour ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgSubmitMisbehaviour ToProto is not implemented.");
    }
}

/// <summary>
/// MsgCreateClient defines a message to create an IBC client
/// </summary>
public class MsgCreateClient : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgCreateClient;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgCreateClient ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgCreateClient ToProto is not implemented.");
    }
}

