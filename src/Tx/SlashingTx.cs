namespace SecretNET.Tx;

public class SlashingTx
{
    private TxClient _tx;

    internal SlashingTx(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// MsgUnjail defines a message to release a validator from jail.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Slashing.V1Beta1.MsgUnjailResponse>> Unjail(Cosmos.Slashing.V1Beta1.MsgUnjail msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Cosmos.Slashing.V1Beta1.MsgUnjailResponse>(txResult) : null;
    }

}
    