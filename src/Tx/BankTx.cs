using System.Net;

namespace SecretNET.Tx;

public class BankTx
{
    private TxClient _tx;

    internal BankTx(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// MsgSend represents a message to send coins from one account to another.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Bank.V1Beta1.MsgSendResponse>> Send(Cosmos.Bank.V1Beta1.MsgSend msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Bank.V1Beta1.MsgSendResponse>(txResult);
    }

    /// <summary>
    /// Sends SCRT to the specified to address.
    /// </summary>
    /// <param name="toAddress">To address.</param>
    /// <param name="amount">The amount (1000000 uscrt == 1SCRT).</param>
    /// <param name="denom">The denom, default 'uscrt'</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>SingleSecretTx&lt;Cosmos.Bank.V1Beta1.MsgSendResponse&gt;.</returns>
    public async Task<SingleSecretTx<Cosmos.Bank.V1Beta1.MsgSendResponse>> Send(string toAddress, int amount, string denom = "uscrt", TxOptions txOptions = null)
    {
        var msgSend = new Cosmos.Bank.V1Beta1.MsgSend()
        {
            FromAddress = _tx.WalletAddress,
            ToAddress = toAddress,
        };
        msgSend.Amount.Add(new Cosmos.Base.V1Beta1.Coin() { Amount = amount.ToString(), Denom = denom });

        var txResult = await _tx.Broadcast(msgSend, txOptions);
        return new SingleSecretTx<Cosmos.Bank.V1Beta1.MsgSendResponse>(txResult);
    }

    /// <summary>
    /// MsgMultiSend represents an arbitrary multi-in, multi-out send message.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Bank.V1Beta1.MsgMultiSendResponse>> MultiSend(Cosmos.Bank.V1Beta1.MsgMultiSend msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Bank.V1Beta1.MsgMultiSendResponse>(txResult);
    }

}
