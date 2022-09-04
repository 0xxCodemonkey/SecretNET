namespace SecretNET.Tx;

public class StakingTxSimulate
{
    private TxClient _tx;

    internal StakingTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Simulates "MsgBeginRedelegate defines an SDK message for performing a redelegation of coins from a delegator and source validator to a destination validator".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> BeginRedelegate(MsgBeginRedelegate msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgCreateValidator defines an SDK message for creating a new validator".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> CreateValidator(MsgCreateValidator msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgDelegate defines an SDK message for performing a delegation of coins from a delegator to a validator".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> Delegate (MsgDelegate msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgEditValidator defines an SDK message for editing an existing validator".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> EditValidator(MsgEditValidator msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgUndelegate defines an SDK message for performing an undelegation from a delegate and a validator"
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> Undelegate(MsgUndelegate msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

}
    