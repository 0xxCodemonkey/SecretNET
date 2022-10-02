namespace SecretNET.Tx;

public class AuthzTx
{
    private TxClient _tx;

    internal AuthzTx(TxClient tx)
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
    public async Task<SingleSecretTx<Cosmos.Authz.V1Beta1.MsgExecResponse>> Exec(Cosmos.Authz.V1Beta1.MsgExec msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Cosmos.Authz.V1Beta1.MsgExecResponse>(txResult) : null;
    }

    /// <summary>
    /// MsgGrant is a request type for Grant method. It declares authorization to the grantee
    /// on behalf of the granter with the provided expiration time.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Authz.V1Beta1.MsgGrantResponse>> Grant(Cosmos.Authz.V1Beta1.MsgGrant msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Cosmos.Authz.V1Beta1.MsgGrantResponse>(txResult) : null;
    }

    /// <summary>
    /// MsgRevoke revokes any authorization with the provided sdk.Msg type on the
    /// granter's account with that has been granted to the grantee.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Authz.V1Beta1.MsgRevokeResponse>> Revoke(Cosmos.Authz.V1Beta1.MsgRevoke msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Cosmos.Authz.V1Beta1.MsgRevokeResponse>(txResult) : null;
    }

}
