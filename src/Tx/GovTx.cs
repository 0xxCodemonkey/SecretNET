namespace SecretNET.Tx;

public class GovTx
{
    private TxClient _tx;
    private GovTxSimulate _txSimulte;

    internal GovTx(TxClient tx)
    {
        _tx = tx;
        _txSimulte = new GovTxSimulate(tx);
    }

    public GovTxSimulate Simulate
    {
        get { return _txSimulte; }
    }

    /// <summary>
    /// MsgDeposit defines a message to submit a deposit to an existing proposal.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> Deposit(MsgDeposit msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgSubmitProposal defines an sdk.Msg type that supports submitting arbitrary proposal Content.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> SubmitProposal(MsgSubmitProposal msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgVote defines a message to cast a vote.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> Vote(MsgVote msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

    /// <summary>
    /// MsgVoteWeighted defines a message to cast a vote, with an option to split the vote.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SecretTx> VoteWeighted(MsgVoteWeighted msg, TxOptions? txOptions = null)
    {
        return await _tx.Broadcast(msg, txOptions);
    }

}
    