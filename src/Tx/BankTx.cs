namespace SecretNET.Tx;

public class BankTx
{
    private TxClient _tx;
    private BankTxSimulate _txSimulte;

    internal BankTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new BankTxSimulate(tx);
    }

    public BankTxSimulate Simulate
    {
        get { return _txSimulte; }
    }

    /// <summary>
    /// MsgSend represents a message to send coins from one account to another.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> Send(MsgSend msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgMultiSend represents an arbitrary multi-in, multi-out send message.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> MultiSend(MsgMultiSend msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

}
