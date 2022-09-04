namespace SecretNET.Tx;

public class CrisisTx
{
    private TxClient _tx;
    private CrisisTxSimulate _txSimulte;

    internal CrisisTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new CrisisTxSimulate(tx);
    }

    public CrisisTxSimulate Simulate
    {
        get { return _txSimulte; }
    }

    /// <summary>
    /// MsgVerifyInvariant represents a message to verify a particular invariance.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> VerifyInvariant(MsgVerifyInvariant msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

}
    