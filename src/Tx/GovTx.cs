namespace SecretNET.Tx;

public class GovTx
{
    private TxClient _tx;

    internal GovTx(TxClient tx)
    {
        _tx = tx;
    }

    /// <summary>
    /// MsgDeposit defines a message to submit a deposit to an existing proposal.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Gov.V1Beta1.MsgDepositResponse>> Deposit(Cosmos.Gov.V1Beta1.MsgDeposit msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Gov.V1Beta1.MsgDepositResponse>(txResult);
    }

    /// <summary>
    /// MsgSubmitProposal defines an sdk.Msg type that supports submitting arbitrary proposal Content.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Gov.V1Beta1.MsgSubmitProposalResponse>> SubmitProposal(Cosmos.Gov.V1Beta1.MsgSubmitProposal msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Gov.V1Beta1.MsgSubmitProposalResponse>(txResult);
    }

    /// <summary>
    /// MsgVote defines a message to cast a vote.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Gov.V1Beta1.MsgVoteResponse>> Vote(Cosmos.Gov.V1Beta1.MsgVote msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Gov.V1Beta1.MsgVoteResponse>(txResult);
    }

    /// <summary>
    /// MsgVoteWeighted defines a message to cast a vote, with an option to split the vote.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    public async Task<SingleSecretTx<Cosmos.Gov.V1Beta1.MsgVoteWeightedResponse>> VoteWeighted(Cosmos.Gov.V1Beta1.MsgVoteWeighted msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return new SingleSecretTx<Cosmos.Gov.V1Beta1.MsgVoteWeightedResponse>(txResult);
    }

}
    