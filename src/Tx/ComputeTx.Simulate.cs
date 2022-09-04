namespace SecretNET.Tx;

public class ComputeTxSimulate
{
    private TxClient _tx;

    internal ComputeTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Simulates "Execute a function on a contract".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> ExecuteContract(MsgExecuteContract msg, TxOptions txOptions = null)
    {
        if (string.IsNullOrEmpty(msg.Sender))
        {
            msg.Sender = _tx.secretClient.WalletAddress;
        }
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulate: Execute a function on a contract
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> ExecuteContract<T>(MsgExecuteContract msg, TxOptions txOptions = null)
    {
        return await ExecuteContract(msg, txOptions);
    }

    /// <summary>
    /// Simulates "Upload a compiled contract to Secret Network".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> StoreCode(MsgStoreCode msg, TxOptions txOptions = null)
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

        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "Instantiate a contract from code id".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> InstantiateContract(MsgInstantiateContract msg, TxOptions txOptions = null)
    {
        if (string.IsNullOrEmpty(msg.Sender))
        {
            msg.Sender = _tx.secretClient.WalletAddress;
        }

        return await _tx.Simulate(msg, txOptions);
    }

}
