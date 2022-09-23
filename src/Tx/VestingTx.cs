namespace SecretNET.Tx;

public class VestingTx
{
    private TxClient _tx;

    internal VestingTx(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// MsgCreateVestingAccount defines a message that enables creating a vesting account.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Vesting.V1Beta1.MsgCreateVestingAccountResponse>> CreateVestingAccount(Cosmos.Vesting.V1Beta1.MsgCreateVestingAccount msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Vesting.V1Beta1.MsgCreateVestingAccountResponse>(txResult);
    }

}
    