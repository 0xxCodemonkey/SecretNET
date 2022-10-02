namespace SecretNET.Tx;

public class ComputeTx
{
    private TxClient _tx;

    internal ComputeTx(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Execute a function on a contract
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> ExecuteContract(MsgExecuteContract msg, TxOptions txOptions = null)
    {
        if (string.IsNullOrWhiteSpace(msg.Sender))
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
    public async Task<SingleSecretTx<Secret.Compute.V1Beta1.MsgStoreCodeResponse>> StoreCode(MsgStoreCode msg, TxOptions txOptions = null)
    {
        if (msg == null || msg.WasmByteCode == null || msg.WasmByteCode.Length == 0) return null;

        if (!await msg.WasmByteCode.IsGzip())
        {
            msg.WasmByteCode = await msg.WasmByteCode.CompressGzip();
        }

        if (string.IsNullOrWhiteSpace(msg.Sender))
        {
            msg.Sender = _tx.secretClient.WalletAddress;
        }

        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Secret.Compute.V1Beta1.MsgStoreCodeResponse>(txResult) : null;
    }

    /// <summary>
    /// Instantiate a contract from code id.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Secret.Compute.V1Beta1.MsgInstantiateContractResponse>> InstantiateContract(MsgInstantiateContract msg, TxOptions txOptions = null)
    {
        if (string.IsNullOrWhiteSpace(msg.Sender))
        {
            msg.Sender = _tx.secretClient.WalletAddress;
        }

        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Secret.Compute.V1Beta1.MsgInstantiateContractResponse>(txResult) : null;
    }

}
