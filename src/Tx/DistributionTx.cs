namespace SecretNET.Tx;

public class DistributionTx
{
    private TxClient _tx;
    private DistributionTxSimulate _txSimulte;

    internal DistributionTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new DistributionTxSimulate(tx);
    }

    public DistributionTxSimulate Simulate
    {
        get { return _txSimulte; }
    }

    /// <summary>
    /// MsgFundCommunityPool allows an account to directly fund the community pool.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> FundCommunityPool(MsgFundCommunityPool msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgSetWithdrawAddress sets the withdraw address for a delegator (or validator self-delegation).
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> SetWithdrawAddress(MsgSetWithdrawAddress msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgWithdrawDelegatorReward represents delegation withdrawal to a delegator from a single validator.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> WithdrawDelegatorReward(MsgWithdrawDelegatorReward msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgWithdrawValidatorCommission withdraws the full commission to the validator address.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> WithdrawValidatorCommission(MsgWithdrawValidatorCommission msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

}
    