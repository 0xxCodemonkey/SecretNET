namespace SecretNET.Tx;

public class ComputeTx
{
    private TxClient _tx;
    private ComputeTxSimulate _txSimulte;

    internal ComputeTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new ComputeTxSimulate(tx);
    }

    public ComputeTxSimulate Simulate
    {
        get { return _txSimulte; }
    }


    /// <summary>
    /// Execute a function on a contract
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> ExecuteContract(MsgExecuteContract msg, TxOptions txOptions = null)
    {
        if (string.IsNullOrEmpty(msg.Sender))
        {
            msg.Sender = _tx.secretClient.WalletAddress;
        }
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// Execute a function on a contract
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<T>> ExecuteContract<T>(MsgExecuteContract msg, TxOptions txOptions = null)
    {
        var result = await ExecuteContract(msg, txOptions);
        if (result != null)
        {
            return new SingleSecretTx<T>(result);
        }
        return null;
    }

    /// <summary>
    /// Upload a compiled contract to Secret Network.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<StoreCodeSecretTx> StoreCode(MsgStoreCode msg, TxOptions txOptions = null)
    {
        if (msg == null || msg.WasmByteCode == null || msg.WasmByteCode.Length == 0) return null;

        if (!await msg.WasmByteCode.IsGzip())
        {
            msg.WasmByteCode = await msg.WasmByteCode.CompressGzip();
        }

        if (string.IsNullOrEmpty(msg.Sender))
        {
            msg.Sender = _tx.secretClient.WalletAddress;
        }

        var secretTx = await _tx.Broadcast(msg, txOptions);
        return new StoreCodeSecretTx(secretTx);
    }

    /// <summary>
    /// Instantiate a contract from code id.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<InstantiateContractSecretTx> InstantiateContract(MsgInstantiateContract msg, TxOptions txOptions = null)
    {
        if (string.IsNullOrEmpty(msg.Sender))
        {
            msg.Sender = _tx.secretClient.WalletAddress;
        }

        var secretTx = await _tx.Broadcast(msg, txOptions);
        return new InstantiateContractSecretTx(secretTx);
    }

}
