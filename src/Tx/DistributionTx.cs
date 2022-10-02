namespace SecretNET.Tx;

public class DistributionTx
{
    private TxClient _tx;
    internal DistributionTx(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// MsgFundCommunityPool allows an account to directly fund the community pool.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Distribution.V1Beta1.MsgFundCommunityPoolResponse>> FundCommunityPool(Cosmos.Distribution.V1Beta1.MsgFundCommunityPool msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Cosmos.Distribution.V1Beta1.MsgFundCommunityPoolResponse>(txResult) : null;
    }

    /// <summary>
    /// MsgSetWithdrawAddress sets the withdraw address for a delegator (or validator self-delegation).
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Distribution.V1Beta1.MsgSetWithdrawAddressResponse>> SetWithdrawAddress(Cosmos.Distribution.V1Beta1.MsgSetWithdrawAddress msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Cosmos.Distribution.V1Beta1.MsgSetWithdrawAddressResponse>(txResult) : null;
    }

    /// <summary>
    /// MsgWithdrawDelegatorReward represents delegation withdrawal to a delegator from a single validator.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Distribution.V1Beta1.MsgWithdrawDelegatorRewardResponse>> WithdrawDelegatorReward(Cosmos.Distribution.V1Beta1.MsgWithdrawDelegatorReward msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Cosmos.Distribution.V1Beta1.MsgWithdrawDelegatorRewardResponse>(txResult) : null;
    }

    /// <summary>
    /// MsgWithdrawValidatorCommission withdraws the full commission to the validator address.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Distribution.V1Beta1.MsgWithdrawValidatorCommissionResponse>> WithdrawValidatorCommission(Cosmos.Distribution.V1Beta1.MsgWithdrawValidatorCommission msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Cosmos.Distribution.V1Beta1.MsgWithdrawValidatorCommissionResponse>(txResult) : null;
    }

}
    