namespace SecretNET.Tx;

public class GovTxSimulate
{
    private TxClient _tx;

    internal GovTxSimulate(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// Simulates "MsgDeposit defines a message to submit a deposit to an existing proposal".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> Deposit(MsgDeposit msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgSubmitProposal defines an sdk.Msg type that supports submitting arbitrary proposal Content".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> SubmitProposal(MsgSubmitProposal msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgVote defines a message to cast a vote.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> Vote(MsgVote msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

    /// <summary>
    /// Simulates "MsgVoteWeighted defines a message to cast a vote, with an option to split the vote".
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SimulateResponse> VoteWeighted(MsgVoteWeighted msg, TxOptions? txOptions = null)
    {
        return await _tx.Simulate(msg, txOptions);
    }

}
    