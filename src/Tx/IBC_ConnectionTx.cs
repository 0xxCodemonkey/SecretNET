namespace SecretNET.Tx;

public class IbcConnectionTx
{
    private TxClient _tx;

    internal IbcConnectionTx(TxClient tx)
    {
        _tx = tx;
    }

    public async Task<SingleSecretTx<Ibc.Core.Connection.V1.MsgConnectionOpenInitResponse>> ConnectionOpenInit(Ibc.Core.Connection.V1.MsgConnectionOpenInit msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Ibc.Core.Connection.V1.MsgConnectionOpenInitResponse>(txResult) : null;
    }


    public async Task<SingleSecretTx<Ibc.Core.Connection.V1.MsgConnectionOpenTryResponse>> ConnectionOpenTry(Ibc.Core.Connection.V1.MsgConnectionOpenTry msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Ibc.Core.Connection.V1.MsgConnectionOpenTryResponse>(txResult) : null;
    }

    public async Task<SingleSecretTx<Ibc.Core.Connection.V1.MsgConnectionOpenAckResponse>> ConnectionOpenAck(Ibc.Core.Connection.V1.MsgConnectionOpenAck msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Ibc.Core.Connection.V1.MsgConnectionOpenAckResponse>(txResult) : null;
    }

    public async Task<SingleSecretTx<Ibc.Core.Connection.V1.MsgConnectionOpenConfirmResponse>> ConnectionOpenConfirm(Ibc.Core.Connection.V1.MsgConnectionOpenConfirm msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Ibc.Core.Connection.V1.MsgConnectionOpenConfirmResponse>(txResult) : null;
    }

}
    