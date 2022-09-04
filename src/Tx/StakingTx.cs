namespace SecretNET.Tx;

public class StakingTx
{
    private TxClient _tx;
    private StakingTxSimulate _txSimulte;

    internal StakingTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new StakingTxSimulate(tx);
    }

    public StakingTxSimulate Simulate
    {
        get { return _txSimulte; }
    }

    /// <summary>
    /// MsgBeginRedelegate defines an SDK message for performing a redelegation of coins from a delegator and source validator to a destination validator.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> BeginRedelegate(MsgBeginRedelegate msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgCreateValidator defines an SDK message for creating a new validator.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> CreateValidator(MsgCreateValidator msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgDelegate defines an SDK message for performing a delegation of coins from a delegator to a validator.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> Delegate (MsgDelegate msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgEditValidator defines an SDK message for editing an existing validator.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> EditValidator(MsgEditValidator msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgUndelegate defines an SDK message for performing an undelegation from a delegate and a validator
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> Undelegate(MsgUndelegate msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

}
    