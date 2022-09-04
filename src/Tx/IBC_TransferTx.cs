namespace SecretNET.Tx;

public class IbcTransferTx
{
    private TxClient _tx;
    private IbcTransferTxSimulate _txSimulte;

    internal IbcTransferTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new IbcTransferTxSimulate(tx);
    }

    public IbcTransferTxSimulate Simulate
    {
        get { return _txSimulte; }
    }

    /// <summary>
    /// MsgTransfer defines a msg to transfer fungible tokens (i.e Coins) between ICS20 enabled chains.
    /// See ICS Spec here:
    /// https://github.com/cosmos/ics/tree/master/spec/ics-020-fungible-token-transfer#data-structures
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> Transfer(MsgTransfer msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

}
    