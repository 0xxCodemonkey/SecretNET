namespace SecretNET.Tx;

public class SlashingTx
{
    private TxClient _tx;
    private SlashingTxSimulate _txSimulte;

    internal SlashingTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new SlashingTxSimulate(tx);
    }

    public SlashingTxSimulate Simulate
    {
        get { return _txSimulte; }
    }

    /// <summary>
    /// MsgUnjail defines a message to release a validator from jail.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> Unjail(MsgUnjail msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

}
    