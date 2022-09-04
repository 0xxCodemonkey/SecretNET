namespace SecretNET.Tx;

public class VestingTxSimulate
{
    private TxClient _tx;

    internal VestingTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Simulates "MsgCreateVestingAccount defines a message that enables creating a vesting account".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> CreateVestingAccount(MsgCreateVestingAccount msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

}
    