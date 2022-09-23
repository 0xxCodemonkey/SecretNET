namespace SecretNET.Tx;

public class StakingTx
{
    private TxClient _tx;

    internal StakingTx(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// MsgBeginRedelegate defines an SDK message for performing a redelegation of coins from a delegator and source validator to a destination validator.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Staking.V1Beta1.MsgBeginRedelegateResponse>> BeginRedelegate(Cosmos.Staking.V1Beta1.MsgBeginRedelegate msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Staking.V1Beta1.MsgBeginRedelegateResponse>(txResult);
    }

    /// <summary>
    /// MsgCreateValidator defines an SDK message for creating a new validator.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Staking.V1Beta1.MsgCreateValidatorResponse>> CreateValidator(Cosmos.Staking.V1Beta1.MsgCreateValidator msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Staking.V1Beta1.MsgCreateValidatorResponse>(txResult);
    }

    /// <summary>
    /// MsgDelegate defines an SDK message for performing a delegation of coins from a delegator to a validator.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Staking.V1Beta1.MsgDelegateResponse>> Delegate (Cosmos.Staking.V1Beta1.MsgDelegate msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Staking.V1Beta1.MsgDelegateResponse>(txResult);
    }

    /// <summary>
    /// MsgEditValidator defines an SDK message for editing an existing validator.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Staking.V1Beta1.MsgEditValidatorResponse>> EditValidator(Cosmos.Staking.V1Beta1.MsgEditValidator msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Staking.V1Beta1.MsgEditValidatorResponse>(txResult);
    }

    /// <summary>
    /// MsgUndelegate defines an SDK message for performing an undelegation from a delegate and a validator
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Staking.V1Beta1.MsgUndelegateResponse>> Undelegate(Cosmos.Staking.V1Beta1.MsgUndelegate msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Staking.V1Beta1.MsgUndelegateResponse>(txResult);
    }

}
    