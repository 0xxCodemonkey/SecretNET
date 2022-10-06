using SecretNET.Crypto.Secp256k1;

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
        return txResult != null ? new SingleSecretTx<Cosmos.Gov.V1Beta1.MsgDepositResponse>(txResult) : null;
    }

    /// <summary>
    /// MsgSubmitProposal defines an sdk.Msg type that supports submitting arbitrary proposal Content.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="txOptions"></param>
    /// <returns></returns>
    private async Task<SingleSecretTx<Cosmos.Gov.V1Beta1.MsgSubmitProposalResponse>> SubmitProposal(Cosmos.Gov.V1Beta1.MsgSubmitProposal msg, TxOptions txOptions = null)
    {
        var txResult = await _tx.Broadcast(msg, txOptions);
        return txResult != null ? new SingleSecretTx<Cosmos.Gov.V1Beta1.MsgSubmitProposalResponse>(txResult) : null;
    }

    /// <summary>
    /// Submits a typed proposal.
    /// </summary>
    /// <param name="proposal">The proposal.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>SingleSecretTx&lt;Cosmos.Gov.V1Beta1.MsgSubmitProposalResponse&gt;.</returns>
    public async Task<SingleSecretTx<Cosmos.Gov.V1Beta1.MsgSubmitProposalResponse>> SubmitProposal(ProposalBaseClass proposal, TxOptions txOptions = null)
    {
        var msgSubmitProposal = new Cosmos.Gov.V1Beta1.MsgSubmitProposal()
        {
            Proposer = _tx.Wallet.Address,
            Content = proposal.AsProposalContent(),
        };
        if (proposal.InitialDeposit != null && proposal.InitialDeposit.Any())
        {
            msgSubmitProposal.InitialDeposit.Add(proposal.InitialDeposit.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());
        }
        return await SubmitProposal(msgSubmitProposal, txOptions);
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
        return txResult != null ? new SingleSecretTx<Cosmos.Gov.V1Beta1.MsgVoteResponse>(txResult) : null;
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
        return txResult != null ? new SingleSecretTx<Cosmos.Gov.V1Beta1.MsgVoteWeightedResponse>(txResult) : null;
    }

}
    