namespace SecretNET.Tx;

public class AuthzTx
{
    private TxClient _tx;
    private AuthzTxSimulate _txSimulte;

    internal AuthzTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new AuthzTxSimulate(tx);
    }

    public AuthzTxSimulate Simulate
    {
        get { return _txSimulte; }
    }

    /// <summary>
    /// MsgExec attempts to execute the provided messages using
    /// authorizations granted to the grantee. Each message should have only
    /// one signer corresponding to the granter of the authorization.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> Exec(MsgExec msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgGrant is a request type for Grant method. It declares authorization to the grantee
    /// on behalf of the granter with the provided expiration time.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> Grant(MsgGrant msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgRevoke revokes any authorization with the provided sdk.Msg type on the
    /// granter's account with that has been granted to the grantee.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> Revoke(MsgRevoke msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

}
