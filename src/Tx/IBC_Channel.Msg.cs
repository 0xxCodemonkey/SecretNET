namespace SecretNET.Tx;

/// <summary>
/// MsgRecvPacket receives incoming IBC packet
/// </summary>
public class MsgRecvPacket : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgRecvPacket;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgSubmitEvidence ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgSubmitEvidence ToProto is not implemented.");
    }
}

/// <summary>
/// MsgTimeout receives timed-out packet
/// </summary>
public class MsgTimeout : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgTimeout;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgTimeout ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgTimeout ToProto is not implemented.");
    }
}

/// <summary>
/// MsgTimeoutOnClose timed-out packet upon counterparty channel closure.
/// </summary>
public class MsgTimeoutOnClose : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgTimeoutOnClose;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgTimeoutOnClose ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgTimeoutOnClose ToProto is not implemented.");
    }
}

/// <summary>
/// MsgChannelOpenInit defines an sdk.Msg to initialize a channel handshake. It is called by a relayer on Chain A.
/// </summary>
public class MsgChannelOpenInit : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgChannelOpenInit;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgChannelOpenInit ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgChannelOpenInit ToProto is not implemented.");
    }
}

/// <summary>
/// MsgAcknowledgement receives incoming IBC acknowledgement
/// </summary>
public class MsgAcknowledgement : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgAcknowledgement;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgAcknowledgement ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgAcknowledgement ToProto is not implemented.");
    }
}

/// <summary>
/// MsgChannelOpenInit defines a msg sent by a Relayer to try to open a channel on Chain B.
/// </summary>
public class MsgChannelOpenTry : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgChannelOpenTry;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgChannelOpenTry ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgChannelOpenTry ToProto is not implemented.");
    }
}

/// <summary>
/// MsgChannelOpenAck defines a msg sent by a Relayer to Chain A to acknowledge the change of channel state to TRYOPEN on Chain B.
/// </summary>
public class MsgChannelOpenAck : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgChannelOpenAck;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgChannelOpenAck ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgChannelOpenAck ToProto is not implemented.");
    }
}

/// <summary>
/// MsgChannelOpenConfirm defines a msg sent by a Relayer to Chain B to acknowledge the change of channel state to OPEN on Chain A.
/// </summary>
public class MsgChannelOpenConfirm : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgChannelOpenConfirm;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgChannelOpenConfirm ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgChannelOpenConfirm ToProto is not implemented.");
    }
}

/// <summary>
/// MsgChannelCloseInit defines a msg sent by a Relayer to Chain A to close a channel with Chain B.
/// </summary>
public class MsgChannelCloseInit : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgChannelCloseInit;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgChannelCloseInit ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgChannelCloseInit ToProto is not implemented.");
    }
}

/// <summary>
/// MsgChannelCloseConfirm defines a msg sent by a Relayer to Chain B to acknowledge the change of channel state to CLOSED on Chain A.
/// </summary>
public class MsgChannelCloseConfirm : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgChannelCloseConfirm;

    public override Task<IMessage> ToProto()
    {
        throw new NotImplementedException("MsgChannelCloseConfirm ToAmino is not implemented.");
    }
    public override Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgChannelCloseConfirm ToProto is not implemented.");
    }
}