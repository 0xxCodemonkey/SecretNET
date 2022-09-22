namespace SecretNET.Tx;

public class IbcTransferTx
{
    private TxClient _tx;

    internal IbcTransferTx(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// MsgTransfer defines a msg to transfer fungible tokens (i.e Coins) between ICS20 enabled chains.
    /// See ICS Spec here:
    /// https://github.com/cosmos/ics/tree/master/spec/ics-020-fungible-token-transfer#data-structures
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Ibc.Applications.Transfer.V1.MsgTransferResponse>> Transfer(Ibc.Applications.Transfer.V1.MsgTransfer msg, TxOptions? txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Ibc.Applications.Transfer.V1.MsgTransferResponse>(txResult);
    }

}
    