namespace SecretNET.Tx;

public class CrisisTx
{
    private TxClient _tx;

    internal CrisisTx(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// MsgVerifyInvariant represents a message to verify a particular invariance.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Crisis.V1Beta1.MsgVerifyInvariantResponse>> VerifyInvariant(Cosmos.Crisis.V1Beta1.MsgVerifyInvariant msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Crisis.V1Beta1.MsgVerifyInvariantResponse>(txResult);
    }

}
    