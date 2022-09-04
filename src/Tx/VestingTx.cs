namespace SecretNET.Tx;

public class VestingTx
{
    private TxClient _tx;
    private VestingTxSimulate _txSimulte;

    internal VestingTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new VestingTxSimulate(tx);
    }

    public VestingTxSimulate Simulate
    {
        get { return _txSimulte; }
    }

    /// <summary>
    /// MsgCreateVestingAccount defines a message that enables creating a vesting account.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> CreateVestingAccount(MsgCreateVestingAccount msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

}
    