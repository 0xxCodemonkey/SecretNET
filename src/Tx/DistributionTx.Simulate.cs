namespace SecretNET.Tx;

public class DistributionTxSimulate
{
    private TxClient _tx;

    internal DistributionTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Simulates "MsgFundCommunityPool allows an account to directly fund the community pool".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> FundCommunityPool(MsgFundCommunityPool msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgSetWithdrawAddress sets the withdraw address for a delegator (or validator self-delegation)".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> SetWithdrawAddress(MsgSetWithdrawAddress msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgWithdrawDelegatorReward represents delegation withdrawal to a delegator from a single validator".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> WithdrawDelegatorReward(MsgWithdrawDelegatorReward msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgWithdrawValidatorCommission withdraws the full commission to the validator address".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> WithdrawValidatorCommission(MsgWithdrawValidatorCommission msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

}
    