namespace SecretNET.Tx;

public class IbcChannelTx
{
    private TxClient _tx;

    internal IbcChannelTx(TxClient tx)
    {
        _tx = tx;
    }

    public async Task<SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelOpenInitResponse>> ChannelOpenInit(Ibc.Core.Channel.V1.MsgChannelOpenInit msg, TxOptions? txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelOpenInitResponse>(txResult);
    }

    public async Task<SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelOpenTryResponse>> ChannelOpenTry(Ibc.Core.Channel.V1.MsgChannelOpenTry msg, TxOptions? txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelOpenTryResponse>(txResult);
    }

    public async Task<SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelOpenAckResponse>> ChannelOpenAck(Ibc.Core.Channel.V1.MsgChannelOpenAck msg, TxOptions? txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelOpenAckResponse>(txResult);
    }

    public async Task<SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelOpenConfirmResponse>> ChannelOpenConfirm(Ibc.Core.Channel.V1.MsgChannelOpenConfirm msg, TxOptions? txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelOpenConfirmResponse>(txResult);
    }

    public async Task<SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelCloseInitResponse>> ChannelCloseInit(Ibc.Core.Channel.V1.MsgChannelCloseInit msg, TxOptions? txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelCloseInitResponse>(txResult);
    }

    public async Task<SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelCloseConfirmResponse>> ChannelCloseConfirm(Ibc.Core.Channel.V1.MsgChannelCloseConfirm msg, TxOptions? txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Ibc.Core.Channel.V1.MsgChannelCloseConfirmResponse>(txResult);
    }

    public async Task<SingleSecretTx<Ibc.Core.Channel.V1.MsgRecvPacketResponse>> RecvPacket(Ibc.Core.Channel.V1.MsgRecvPacket msg, TxOptions? txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Ibc.Core.Channel.V1.MsgRecvPacketResponse>(txResult);
    }

    public async Task<SingleSecretTx<Ibc.Core.Channel.V1.MsgTimeoutResponse>> Timeout(Ibc.Core.Channel.V1.MsgTimeout msg, TxOptions? txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Ibc.Core.Channel.V1.MsgTimeoutResponse>(txResult);
    }

    public async Task<SingleSecretTx<Ibc.Core.Channel.V1.MsgTimeoutOnCloseResponse>> TimeoutOnClose(Ibc.Core.Channel.V1.MsgTimeoutOnClose msg, TxOptions? txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Ibc.Core.Channel.V1.MsgTimeoutOnCloseResponse>(txResult);
    }

    public async Task<SingleSecretTx<Ibc.Core.Channel.V1.MsgAcknowledgementResponse>> Acknowledgement(Ibc.Core.Channel.V1.MsgAcknowledgement msg, TxOptions? txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Ibc.Core.Channel.V1.MsgAcknowledgementResponse>(txResult);
    }

}
    