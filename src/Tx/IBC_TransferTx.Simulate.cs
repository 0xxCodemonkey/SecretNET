namespace SecretNET.Tx;

public class IbcTransferTxSimulate
{
    private TxClient _tx;

    internal IbcTransferTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Simulates "MsgTransfer defines a msg to transfer fungible tokens (i.e Coins) between ICS20 enabled chains".
    /// See ICS Spec here:
    /// https://github.com/cosmos/ics/tree/master/spec/ics-020-fungible-token-transfer#data-structures
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> Transfer(MsgTransfer msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

}
    