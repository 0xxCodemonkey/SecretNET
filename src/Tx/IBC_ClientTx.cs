namespace SecretNET.Tx;

public class IbcClientTx
{
    private TxClient _tx;

    internal IbcClientTx(TxClient tx)
    {
        _tx = tx;
    }

    public async Task<SingleSecretTx<Ibc.Core.Client.V1.MsgCreateClientResponse>> CreateClient(Ibc.Core.Client.V1.MsgCreateClient msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Ibc.Core.Client.V1.MsgCreateClientResponse>(txResult) : null;
    }


    public async Task<SingleSecretTx<Ibc.Core.Client.V1.MsgUpdateClientResponse>> UpdateClient(Ibc.Core.Client.V1.MsgUpdateClient msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Ibc.Core.Client.V1.MsgUpdateClientResponse>(txResult) : null;
    }

    public async Task<SingleSecretTx<Ibc.Core.Client.V1.MsgUpgradeClientResponse>> UpgradeClient(Ibc.Core.Client.V1.MsgUpgradeClient msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Ibc.Core.Client.V1.MsgUpgradeClientResponse>(txResult) : null;
    }

    public async Task<SingleSecretTx<Ibc.Core.Client.V1.MsgSubmitMisbehaviourResponse>> SubmitMisbehaviour(Ibc.Core.Client.V1.MsgSubmitMisbehaviour msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Ibc.Core.Client.V1.MsgSubmitMisbehaviourResponse>(txResult) : null;
    }

}
    