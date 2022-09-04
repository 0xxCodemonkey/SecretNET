namespace SecretNET.Tx;

public class FeegrantTxSimulate
{
    private TxClient _tx;

    internal FeegrantTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Simulates "MsgGrantAllowance adds permission for Grantee to spend up to Allowance of fees from the account of Granter".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> GrantAllowance(MsgGrantAllowance msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgRevokeAllowance removes any existing Allowance from Granter to Grantee".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> RevokeAllowance(MsgRevokeAllowance msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

}
    