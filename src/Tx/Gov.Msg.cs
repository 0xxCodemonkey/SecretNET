namespace SecretNET.Tx;

/// <summary>
/// ProposalStatus enumerates the valid statuses of a proposal.
/// </summary>
public enum ProposalStatus
{
    /// <summary>
    /// PROPOSAL_STATUS_UNSPECIFIED defines the default propopsal status.
    /// </summary>
    PROPOSAL_STATUS_UNSPECIFIED = 0,

    /// <summary>
    /// PROPOSAL_STATUS_DEPOSIT_PERIOD defines a proposal status during the deposit period.
    /// </summary>
    PROPOSAL_STATUS_DEPOSIT_PERIOD = 1,

    /// <summary>
    /// PROPOSAL_STATUS_VOTING_PERIOD defines a proposal status during the voting period.
    /// </summary>
    PROPOSAL_STATUS_VOTING_PERIOD = 2,

    /// <summary>
    /// PROPOSAL_STATUS_PASSED defines a proposal status of a proposal that has passed.
    /// </summary>
    PROPOSAL_STATUS_PASSED = 3,

    /// <summary>
    /// PROPOSAL_STATUS_REJECTED defines a proposal status of a proposal that has been rejected.
    /// </summary>
    PROPOSAL_STATUS_REJECTED = 4,

    /// <summary>
    /// PROPOSAL_STATUS_FAILED defines a proposal status of a proposal that has failed.
    /// </summary>
    PROPOSAL_STATUS_FAILED = 5,

    UNRECOGNIZED = -1,
}

public enum ProposalType
{
    None = 0,
    TextProposal = 1,
    CommunityPoolSpendProposal = 2,

    /// <summary>
    /// @see {@link https://docs.scrt.network/guides/governance} for possible subspaces, keys and values.
    /// </summary>
    ParameterChangeProposal = 3,

    /// <summary>
    /// Not supported with Amino signer.
    /// </summary>
    ClientUpdateProposal = 4,

    /// <summary>
    /// Not supported with Amino signer.
    /// </summary>
    UpgradeProposal = 5,

    SoftwareUpgradeProposal = 6,
    CancelSoftwareUpgradeProposal = 7,
}

/// <summary>
/// MsgSubmitProposal defines an sdk.Msg type that supports submitting arbitrary proposal Content.
/// </summary>
public class MsgSubmitProposal : MsgBase
{
    /// <summary>
    /// Only following message types allowed:
    /// - cosmos.gov.v1beta1.gov.TextProposal
    /// - cosmos.distribution.v1beta1.CommunityPoolSpendProposal
    /// - cosmos.params.v1beta1.ParameterChangeProposal
    /// - ibc.core.client.v1.ClientUpdateProposal
    /// - ibc.core.client.v1.UpgradeProposal
    /// - cosmos.upgrade.v1beta1.SoftwareUpgradeProposal
    /// - cosmos.upgrade.v1beta1.CancelSoftwareUpgradeProposal
    /// </summary>
    private IMessage _content = null;

    public override string MsgType { get; } = MsgGrantAuthorization.MsgSubmitProposal;

    public ProposalType Type { get; set; }    

    public Coin[] InitialDeposit { get; set; }

    public string Proposer { get; set; }


    public MsgSubmitProposal() { }

    public MsgSubmitProposal(ProposalType type, Coin[] initialDeposit, string proposer)
    {
        Type = type;
        InitialDeposit = initialDeposit;
        Proposer = proposer;
    }


    public override async Task<IMessage> ToProto()
    {
        if (Type == ProposalType.TextProposal)
        {
            _content = new Cosmos.Gov.V1Beta1.TextProposal();
        }
        else if (Type == ProposalType.CommunityPoolSpendProposal)
        {
            _content = new Cosmos.Distribution.V1Beta1.CommunityPoolSpendProposal();
        }
        else if (Type == ProposalType.ParameterChangeProposal)
        {
            _content = new Cosmos.Params.V1Beta1.ParameterChangeProposal();
        }
        else if (Type == ProposalType.ClientUpdateProposal)
        {
            _content = new Ibc.Core.Client.V1.ClientUpdateProposal();
        }
        else if (Type == ProposalType.UpgradeProposal)
        {
            _content = new Ibc.Core.Client.V1.UpgradeProposal();
        }
        else if (Type == ProposalType.SoftwareUpgradeProposal)
        {
            _content = new Cosmos.Upgrade.V1Beta1.SoftwareUpgradeProposal();
        }
        else if (Type == ProposalType.CancelSoftwareUpgradeProposal)
        {
            _content = new Cosmos.Upgrade.V1Beta1.CancelSoftwareUpgradeProposal();
        }

        if (_content != null)
        {
            var msgSubmitProposal = new Cosmos.Gov.V1Beta1.MsgSubmitProposal()
            {
                 Content = Any.Pack(_content,""),
                 Proposer = Proposer
            };

            if (InitialDeposit != null)
            {
                msgSubmitProposal.InitialDeposit.Add(InitialDeposit.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());
            }

            return msgSubmitProposal;
        }
        return null;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        throw new NotImplementedException("MsgSubmitProposal ToAmino is not implemented.");
    }
}

/// <summary>
/// MsgVote defines a message to cast a vote.
/// </summary>
public class MsgVote : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgVote;

    public Cosmos.Gov.V1Beta1.VoteOption Option { get; set; }

    public string Voter { get; set; }

    public ulong ProposalId { get; set; }


    public MsgVote() { }

    public MsgVote(Cosmos.Gov.V1Beta1.VoteOption option, string voter, ulong proposalId)
    {
        Option = option;
        Voter = voter;
        ProposalId = proposalId;
    }

    public override async Task<IMessage> ToProto()
    {
        var msgVote = new Cosmos.Gov.V1Beta1.MsgVote()
        {
            Voter = Voter,
            ProposalId = ProposalId,
            Option = Option
        };
        return msgVote;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgVote");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            option = (byte)Option,
            proposal_id = ProposalId,
            voter = Voter
        };
        return aminoMsg;
    }
}

/// <summary>
/// MsgVoteWeighted defines a message to cast a vote, with an option to split the vote.
/// </summary>
public class MsgVoteWeighted : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgVoteWeighted;

    public Cosmos.Gov.V1Beta1.WeightedVoteOption[] Options { get; set; }

    public string Voter { get; set; }

    public ulong ProposalId { get; set; }

    public MsgVoteWeighted() { }

    public MsgVoteWeighted(Cosmos.Gov.V1Beta1.WeightedVoteOption[] options, string voter, ulong proposalId)
    {
        Options = options;
        Voter = voter;
        ProposalId = proposalId;
    }

    public override async Task<IMessage> ToProto()
    {
        var msgVoteWeighted = new Cosmos.Gov.V1Beta1.MsgVoteWeighted()
        {
            Voter = Voter,
            ProposalId = ProposalId,            
        };
        if (Options != null)
        {
            msgVoteWeighted.Options.Add(Options);
        }
        
        return msgVoteWeighted;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgVoteWeighted");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            options = Options.Select(o => new
            {
                option = (byte)o.Option,
                weight = o.Weight
            }),
            proposal_id = ProposalId,
            voter = Voter
        };
        return aminoMsg;
    }
}

/// <summary>
/// MsgDeposit defines a message to submit a deposit to an existing proposal.
/// </summary>
public class MsgDeposit : MsgBase
{
    public override string MsgType { get; } = MsgGrantAuthorization.MsgDeposit;

    public string Depositor { get; set; }

    public ulong ProposalId { get; set; }

    public Coin[] Amount { get; set; }


    public MsgDeposit() { }

    public MsgDeposit(string depositor, ulong proposalId, Coin[] amount)
    {
        Depositor = depositor;
        ProposalId = proposalId;
        Amount = amount;
    }

    public override async Task<IMessage> ToProto()
    {
        var msgDeposit = new Cosmos.Gov.V1Beta1.MsgDeposit()
        {
            Depositor = Depositor,
            ProposalId = ProposalId,
        };
        if (Amount != null)
        {
            msgDeposit.Amount.Add(Amount.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());
        }
        
        return msgDeposit;
    }

    public override async Task<AminoMsg> ToAmino()
    {
        var aminoMsg = new AminoMsg("cosmos-sdk/MsgVoteWeighted");
        // order of properties must be sorted for amino signing!!
        aminoMsg.Value = new
        {
            amount = Amount,
            depositor = Depositor,
            proposal_id = ProposalId,
        };
        return aminoMsg;
    }
}