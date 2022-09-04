namespace SecretNET.Tx;

public class FeegrantTx
{
    private TxClient _tx;
    private FeegrantTxSimulate _txSimulte;

    internal FeegrantTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new FeegrantTxSimulate(tx);
    }

    public FeegrantTxSimulate Simulate
    {
        get { return _txSimulte; }
    }

    /// <summary>
    /// MsgGrantAllowance adds permission for Grantee to spend up to Allowance of fees from the account of Granter.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> GrantAllowance(MsgGrantAllowance msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgRevokeAllowance removes any existing Allowance from Granter to Grantee.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> RevokeAllowance(MsgRevokeAllowance msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

}
    