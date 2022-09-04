namespace SecretNET.Tx;

public class AuthzTxSimulate
{
    private TxClient _tx;

    internal AuthzTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// MsgExec attempts to execute the provided messages using
    /// authorizations granted to the grantee. Each message should have only
    /// one signer corresponding to the granter of the authorization.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> Exec(MsgExec msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// MsgGrant is a request type for Grant method. It declares authorization to the grantee
    /// on behalf of the granter with the provided expiration time.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> Grant(MsgGrant msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// MsgRevoke revokes any authorization with the provided sdk.Msg type on the
    /// granter's account with that has been granted to the grantee.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> Revoke(MsgRevoke msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

}
