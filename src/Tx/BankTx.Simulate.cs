namespace SecretNET.Tx;

public class BankTxSimulate
{
    private TxClient _tx;

    internal BankTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Simulates "MsgSend represents a message to send coins from one account to another".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> Send(MsgSend msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgMultiSend represents an arbitrary multi-in, multi-out send message".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> MultiSend(MsgMultiSend msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

}
