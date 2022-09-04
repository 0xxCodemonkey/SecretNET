namespace SecretNET.Tx;

public class CrisisTxSimulate
{
    private TxClient _tx;

    internal CrisisTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Simulates "MsgVerifyInvariant represents a message to verify a particular invariance".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> VerifyInvariant(MsgVerifyInvariant msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

}
    