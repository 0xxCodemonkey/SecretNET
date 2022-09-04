namespace SecretNET.Tx;

public class SlashingTxSimulate
{
    private TxClient _tx;

    internal SlashingTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Simulates "MsgUnjail defines a message to release a validator from jail".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> Unjail(MsgUnjail msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

}
    