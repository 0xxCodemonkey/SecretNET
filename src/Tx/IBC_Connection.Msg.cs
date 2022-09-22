namespace SecretNET.Tx;

/// <summary>
/// MsgConnectionOpenAck defines a msg sent by a Relayer to Chain A to acknowledge the change of connection state to TRYOPEN on Chain B.
/// </summary>
public class MsgConnectionOpenAck : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgConnectionOpenAck;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgConnectionOpenAck ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgConnectionOpenAck ToProto is not implemented.");
    }
}

/// <summary>
/// MsgConnectionOpenConfirm defines a msg sent by a Relayer to Chain B to acknowledge the change of connection state to OPEN on Chain A.
/// </summary>
public class MsgConnectionOpenConfirm : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgConnectionOpenConfirm;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgConnectionOpenConfirm ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgConnectionOpenConfirm ToProto is not implemented.");
    }
}



