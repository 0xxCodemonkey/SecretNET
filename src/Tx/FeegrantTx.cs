namespace SecretNET.Tx;

public class FeegrantTx
{
    private TxClient _tx;

    internal FeegrantTx(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// MsgGrantAllowance adds permission for Grantee to spend up to Allowance of fees from the account of Granter.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Feegrant.V1Beta1.MsgGrantAllowanceResponse>> GrantAllowance(Cosmos.Feegrant.V1Beta1.MsgGrantAllowance msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Feegrant.V1Beta1.MsgGrantAllowanceResponse>(txResult);
    }

    /// <summary>
    /// MsgRevokeAllowance removes any existing Allowance from Granter to Grantee.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Feegrant.V1Beta1.MsgRevokeAllowanceResponse>> RevokeAllowance(Cosmos.Feegrant.V1Beta1.MsgRevokeAllowance msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Feegrant.V1Beta1.MsgRevokeAllowanceResponse>(txResult);
    }

}
    