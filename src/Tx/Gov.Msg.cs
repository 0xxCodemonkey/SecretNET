namespace SecretNET.Tx;



/// <summary>
/// BaseClass for the Proposal-Content
/// </summary>
public abstract class ProposalBaseClass
{
    /// <summary>
    /// Gets the cosmos message.
    /// </summary>
    /// <value>The cosmos message.</value>
    public IMessage ContentMessage { get; internal set; }

    /// <summary>
    /// Gets or sets the initial deposit.
    /// </summary>
    /// <value>The initial deposit.</value>
    public Coin[] InitialDeposit { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProposalBaseClass"/> class.
    /// </summary>
    /// <param name="initialDeposit">The initial deposit.</param>
    public ProposalBaseClass(Coin[] initialDeposit)
    {
        InitialDeposit = initialDeposit;
    }

    /// <summary>
    /// Converts the proposal for the MsgSubmitProposal.Content (Any) property.
    /// </summary>
    /// <returns>Any.</returns>
    /// <exception cref="System.ArgumentNullException">ProposalBaseClass => CosmosMessage can not be NULL!</exception>
    public Any AsProposalContent()
    {
        if (ContentMessage != null)
        {
            return Google.Protobuf.WellKnownTypes.Any.Pack(ContentMessage, "");
        }
        throw new ArgumentNullException("ProposalBaseClass => CosmosMessage can not be NULL!");
    }
}

/// <summary>
/// TextProposal defines a standard text proposal whose changes need to be manually updated in case of approval.
/// Implements the <see cref="SecretNET.Tx.ProposalBaseClass" />
/// </summary>
/// <seealso cref="SecretNET.Tx.ProposalBaseClass" />
public class TextProposal : ProposalBaseClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextProposal"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="description">The description.</param>
    /// <param name="initialDeposit">The initial deposit.</param>
    public TextProposal(string title, string description, Coin[] initialDeposit):base(initialDeposit)
    {
        ContentMessage = new Cosmos.Gov.V1Beta1.TextProposal() { Title = title, Description = description };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextProposal"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="description">The description.</param>
    /// <param name="initialDeposit">The initial deposit.</param>
    public TextProposal(string title, string description, Coin initialDeposit) : base(new[] { initialDeposit })
    {
        ContentMessage = new Cosmos.Gov.V1Beta1.TextProposal() { Title = title, Description = description };
    }
}

/// <summary>
/// CommunityPoolSpendProposal details a proposal for use of community funds,
/// together with how many coins are proposed to be spent, and to which
/// recipient account.
/// Implements the <see cref="SecretNET.Tx.ProposalBaseClass" />
/// </summary>
/// <seealso cref="SecretNET.Tx.ProposalBaseClass" />
public class CommunityPoolSpendProposal : ProposalBaseClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommunityPoolSpendProposal"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="description">The description.</param>
    /// <param name="recipientAddress">The recipient address.</param>
    /// <param name="amount">The amount.</param>
    /// <param name="initialDeposit">The initial deposit.</param>
    public CommunityPoolSpendProposal(string title, string description, string recipientAddress, Coin[] amount, Coin[] initialDeposit) : base(initialDeposit)
    {
        ContentMessage = new Cosmos.Distribution.V1Beta1.CommunityPoolSpendProposal()
        {
            Title = title,
            Description = description,
            Recipient = recipientAddress,
        };

        if (amount != null && amount.Any())
        {
            ((Cosmos.Distribution.V1Beta1.CommunityPoolSpendProposal)ContentMessage).Amount.Add(amount.Select(c => new Cosmos.Base.V1Beta1.Coin() { Amount = c.Amount, Denom = c.Denom }).ToArray());
        }
    }
}

/// <summary>
/// ParameterChangeProposal defines a proposal to change one or more parameters.
/// Implements the <see cref="SecretNET.Tx.ProposalBaseClass" />
/// </summary>
/// <seealso cref="SecretNET.Tx.ProposalBaseClass" />
public class ParameterChangeProposal : ProposalBaseClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterChangeProposal"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="description">The description.</param>
    /// <param name="changes">The changes.</param>
    /// <param name="initialDeposit">The initial deposit.</param>
    public ParameterChangeProposal(string title, string description, Cosmos.Params.V1Beta1.ParamChange[] changes, Coin[] initialDeposit) : base(initialDeposit)
    {
        ContentMessage = new Cosmos.Params.V1Beta1.ParameterChangeProposal()
        {
            Title= title,
            Description= description,
        };
        foreach (var change in changes)
        {
            ((Cosmos.Params.V1Beta1.ParameterChangeProposal)ContentMessage).Changes.Add(change);
        }
    }
}

/// <summary>
/// SoftwareUpgradeProposal is a gov Content type for initiating a software.
/// Implements the <see cref="SecretNET.Tx.ProposalBaseClass" />
/// </summary>
/// <seealso cref="SecretNET.Tx.ProposalBaseClass" />
public class SoftwareUpgradeProposal : ProposalBaseClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoftwareUpgradeProposal"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="description">The description.</param>
    /// <param name="plan">The plan.</param>
    /// <param name="initialDeposit">The initial deposit.</param>
    public SoftwareUpgradeProposal(string title, string description, Cosmos.Upgrade.V1Beta1.Plan plan, Coin[] initialDeposit) : base(initialDeposit)
    {
        ContentMessage = new Cosmos.Upgrade.V1Beta1.SoftwareUpgradeProposal()
        {
            Title = title,
            Description = description,
            Plan = plan
        };
    }
}

/// <summary>
/// CancelSoftwareUpgradeProposal is a gov Content type for cancelling a software upgrade.
/// Implements the <see cref="SecretNET.Tx.ProposalBaseClass" />
/// </summary>
/// <seealso cref="SecretNET.Tx.ProposalBaseClass" />
public class CancelSoftwareUpgradeProposal : ProposalBaseClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CancelSoftwareUpgradeProposal"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="description">The description.</param>
    /// <param name="initialDeposit">The initial deposit.</param>
    public CancelSoftwareUpgradeProposal(string title, string description, Coin[] initialDeposit) : base(initialDeposit)
    {
        ContentMessage = new Cosmos.Upgrade.V1Beta1.CancelSoftwareUpgradeProposal()
        {
            Title = title,
            Description = description,
        };
    }
}

/// <summary>
/// ClientUpdateProposal is a governance proposal. If it passes, the substitute
/// client's latest consensus state is copied over to the subject client. The proposal
/// handler may fail if the subject and the substitute do not match in client and
/// chain parameters (with exception to latest height, frozen height, and chain-id).
/// Implements the <see cref="SecretNET.Tx.ProposalBaseClass" />
/// </summary>
/// <seealso cref="SecretNET.Tx.ProposalBaseClass" />
public class ClientUpdateProposal : ProposalBaseClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientUpdateProposal"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="description">The description.</param>
    /// <param name="subjectClientId">The subject client identifier.</param>
    /// <param name="substituteClientId">The substitute client identifier.</param>
    /// <param name="initialDeposit">The initial deposit.</param>
    public ClientUpdateProposal(string title, string description, string subjectClientId, string substituteClientId, Coin[] initialDeposit) : base(initialDeposit)
    {
        ContentMessage = new Ibc.Core.Client.V1.ClientUpdateProposal()
        {
            Title = title,
            Description = description,
            SubjectClientId = subjectClientId,
            SubstituteClientId = substituteClientId,
        };
    }
}

/// <summary>
/// UpgradeProposal is a gov Content type for initiating an IBC breaking upgrade.
/// Implements the <see cref="SecretNET.Tx.ProposalBaseClass" />
/// </summary>
/// <seealso cref="SecretNET.Tx.ProposalBaseClass" />
public class UpgradeProposal : ProposalBaseClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpgradeProposal"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="description">The description.</param>
    /// <param name="plan">The plan.</param>
    /// <param name="upgradedClientState">
    /// An UpgradedClientState must be provided to perform an IBC breaking upgrade.
    /// This will make the chain commit to the correct upgraded (self) client state
    /// before the upgrade occurs, so that connecting chains can verify that the
    /// new upgraded client is valid by verifying a proof on the previous version
    /// of the chain. This will allow IBC connections to persist smoothly across
    /// planned chain upgrades
    /// </param>
    /// <param name="initialDeposit">The initial deposit.</param>
    public UpgradeProposal(string title, string description, Cosmos.Upgrade.V1Beta1.Plan plan, Any upgradedClientState, Coin[] initialDeposit) : base(initialDeposit)
    {
        ContentMessage = new Ibc.Core.Client.V1.UpgradeProposal()
        {
            Title = title,
            Description = description,
            Plan = plan,
            UpgradedClientState = upgradedClientState
        };
    }
}



