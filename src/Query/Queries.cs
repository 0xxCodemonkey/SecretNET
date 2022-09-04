namespace SecretNET.Query;

public class Queries : GprcBase
{
    // cosmos
    private AuthQueryClient _authQuery;
    private AuthzQueryClient _authzQuery;
    private BankQueryClient _bankQuery;
    private DistributionQueryClient _distributionQuery;
    private EvidenceQueryClient _evidenceQuery;
    private FeegrantQueryClient _feegrantQuery;
    private GovQueryClient _govQuery;
    private MintQueryClient _mintQuery;
    private ParamsQueryClient _paramsQuery;
    private SlashingQueryClient _slashingQuery;
    private StakingQueryClient _stakingQuery;
    private TendermintQueryClient _tendermintQuery;
    private UpgradeQueryClient _upgradeQuery;

    // IBC
    private IbcChannelQueryClient _ibcChannelQuery;
    private IbcClientQueryClient _ibcClientQuery;
    private IbcConnectionQueryClient _ibcConnectionQuery;
    private IbcTransferQueryClient _ibcTransferQuery;

    // secret Network
    private ComputeQueryClient _computeQuery;
    private RegistrationQueryClient _registrationQuery;

    /// <summary>
    /// Initializes a new instance of the <see cref="Queries" /> class.
    /// </summary>
    /// <param name="secretNetworkClient">The secret network client.</param>
    /// <param name="grpcChannel">The GRPC channel.</param>
    /// <param name="grpcMessageInterceptor">The GRPC message interceptor.</param>
    internal Queries(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
        _authQuery = new AuthQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _authzQuery = new AuthzQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _bankQuery = new BankQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _computeQuery = new ComputeQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _distributionQuery = new DistributionQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _evidenceQuery = new EvidenceQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _feegrantQuery = new FeegrantQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _govQuery = new GovQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _ibcChannelQuery = new IbcChannelQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _ibcClientQuery = new IbcClientQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _ibcConnectionQuery = new IbcConnectionQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _ibcTransferQuery = new IbcTransferQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _mintQuery = new MintQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _paramsQuery = new ParamsQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _registrationQuery = new RegistrationQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _slashingQuery = new SlashingQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _stakingQuery = new StakingQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _tendermintQuery = new TendermintQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
        _upgradeQuery = new UpgradeQueryClient(secretNetworkClient, grpcChannel, grpcMessageInterceptor);
    }

    /// <summary>
    /// AuthQuerier is the query interface for the x/auth module
    /// </summary>
    public AuthQueryClient Auth
    {
        get { return _authQuery; }
    }

    public AuthzQueryClient Authz
    {
        get { return _authzQuery; }
    }

    public BankQueryClient Bank
    {
        get { return _bankQuery; }
    }
    public ComputeQueryClient Compute
    {
        get { return _computeQuery; }
    }

    public DistributionQueryClient Distribution
    {
        get { return _distributionQuery; }
    }

    public EvidenceQueryClient Evidence
    {
        get { return _evidenceQuery; }
    }
    public FeegrantQueryClient Feegrant
    {
        get { return _feegrantQuery; }
    }

    public GovQueryClient Gov
    {
        get { return _govQuery; }
    }    

    public IbcChannelQueryClient IbcChannel
    {
        get { return _ibcChannelQuery; }
    }
    public IbcClientQueryClient IbcClient
    {
        get { return _ibcClientQuery; }
    }
    public IbcConnectionQueryClient IbcConnection
    {
        get { return _ibcConnectionQuery; }
    }

    public IbcTransferQueryClient IbcTransfer
    {
        get { return _ibcTransferQuery; }
    }

    public MintQueryClient Mint
    {
        get { return _mintQuery; }
    }

    public ParamsQueryClient Params
    {
        get { return _paramsQuery; }
    }

    public RegistrationQueryClient Registration
    {
        get { return _registrationQuery; }
    }

    public SlashingQueryClient Slashing
    {
        get { return _slashingQuery; }
    }

    public StakingQueryClient Staking
    {
        get { return _stakingQuery; }
    }

    public TendermintQueryClient Tendermint
    {
        get { return _tendermintQuery; }
    }

    public UpgradeQueryClient Upgrade
    {
        get { return _upgradeQuery; }
    }

}
