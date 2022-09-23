using SecretNET.Query;

namespace SecretNET.Tx;

/// <summary>
/// Provides access to all transaction types / methods.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class TxClient : GprcBase
{
    private Queries _queries;
    private Service.ServiceClient _txClient;

    // Cosmos
    private AuthzTx _authzTx;
    private BankTx _bankTx;
    private CrisisTx _crisisTx;
    private DistributionTx _distributionTx;
    private EvidenceTx _evidenceTx;
    private FeegrantTx _feegrantTx;
    private GovTx _govTx;
    private SlashingTx _slashingTx;
    private StakingTx _stakingTx;
    private VestingTx _vestingTx;

    // IBC
    private IbcChannelTx _ibcChannelTx;
    private IbcClientTx _ibcClientTx;
    private IbcConnectionTx _ibcConnectionTx;
    private IbcTransferTx _ibcTransferTx;

    // Secret Network
    private ComputeTx _computeTx;


    internal TxClient(ISecretNetworkClient secretNetworkClient, Queries queries, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
        _queries = queries;

        // Cosmos
        _authzTx = new AuthzTx(this);
        _bankTx = new BankTx(this);
        _crisisTx = new CrisisTx(this);
        _distributionTx = new DistributionTx(this);
        _evidenceTx = new EvidenceTx(this);
        _feegrantTx = new FeegrantTx(this);
        _govTx = new GovTx(this);
        _slashingTx = new SlashingTx(this);
        _stakingTx = new StakingTx(this); ;
        _vestingTx = new VestingTx(this);

        // IBC
        _ibcChannelTx = new IbcChannelTx(this);
        _ibcClientTx = new IbcClientTx(this);
        _ibcConnectionTx = new IbcConnectionTx(this);
        _ibcTransferTx = new IbcTransferTx(this);

        // Secret Network
        _computeTx = new ComputeTx(this);
    }

    internal Service.ServiceClient ServiceClient
    {
        get
        {
            if (_txClient == null)
            {
                _txClient = grpcMessageInterceptor != null ? new Service.ServiceClient(grpcMessageInterceptor) : new Service.ServiceClient(grpcChannel);
            }
            return _txClient;
        }
    }

    /// <summary>
    /// Authorization module.
    /// </summary>
    /// <value>The authz.</value>
    public AuthzTx Authz
    {
        get { return _authzTx; }
    }

    /// <summary>
    /// Create and broadcast transactions.
    /// </summary>
    /// <value>The bank.</value>
    public BankTx Bank
    {
        get { return _bankTx; }
    }

    /// <summary>
    /// Store, init and execute smart contracts.
    /// </summary>
    /// <value>The compute.</value>
    public ComputeTx Compute
    {
        get { return _computeTx; }
    }

    /// <summary>
    /// Crisis module.
    /// </summary>
    /// <value>The crisis.</value>
    public CrisisTx Crisis
    {
        get { return _crisisTx; }
    }

    /// <summary>
    /// Distribution module.
    /// </summary>
    /// <value>The distribution.</value>
    public DistributionTx Distribution
    {
        get { return _distributionTx; }
    }

    /// <summary>
    /// Evidence module.
    /// </summary>
    /// <value>The evidence.</value>
    public EvidenceTx Evidence
    {
        get { return _evidenceTx; }
    }

    /// <summary>
    /// Feegrant module.
    /// </summary>
    /// <value>The feegrant.</value>
    public FeegrantTx Feegrant
    {
        get { return _feegrantTx; }
    }

    /// <summary>
    /// Governance module.
    /// </summary>
    /// <value>The gov.</value>
    public GovTx Gov
    {
        get { return _govTx; }
    }

    /// <summary>
    /// Slashing module.
    /// </summary>
    /// <value>The slashing.</value>
    public SlashingTx Slashing
    {
        get { return _slashingTx; }
    }

    /// <summary>
    /// Stake module.
    /// </summary>
    /// <value>The staking.</value>
    public StakingTx Staking
    {
        get { return _stakingTx; }
    }

    /// <summary>
    /// Vesting module.
    /// </summary>
    /// <value>The vesting.</value>
    public VestingTx Vesting
    {
        get { return _vestingTx; }
    }

    /// <summary>
    /// IBC Channel module.
    /// </summary>
    /// <value>The ibc channel.</value>
    public IbcChannelTx IbcChannel
    {
        get { return _ibcChannelTx; }
    }

    /// <summary>
    /// IBC Client module.
    /// </summary>
    /// <value>The ibc client.</value>
    public IbcClientTx IbcClient
    {
        get { return _ibcClientTx; }
    }

    /// <summary>
    /// IBC Connection module.
    /// </summary>
    /// <value>The ibc connection.</value>
    public IbcConnectionTx IbcConnection
    {
        get { return _ibcConnectionTx; }
    }

    /// <summary>
    /// IBC Transfer module.
    /// </summary>
    /// <value>The ibc transfer.</value>
    public IbcTransferTx IbcTransfer
    {
        get { return _ibcTransferTx; }
    }

    /// <summary>
    /// Simulates the specified Tx / message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>SimulateResponse.</returns>
    public async Task<SimulateResponse> Simulate(MsgBase message, TxOptions txOptions = null)
    {
        if (message is MsgExecuteContract && string.IsNullOrEmpty(((MsgExecuteContract)message).Sender))
        {
            ((MsgExecuteContract)message).Sender = WalletAddress;
        }
        return await Simulate(new MsgBase[] { message }, txOptions);
    }

    /// <summary>
    /// Simulates the specified Tx / messages.
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>SimulateResponse.</returns>
    public async Task<SimulateResponse> Simulate(MsgBase[] messages, TxOptions txOptions = null)
    {
        foreach (var msg in messages)
        {
            if (msg is MsgExecuteContract && string.IsNullOrEmpty(((MsgExecuteContract)msg).Sender))
            {
                ((MsgExecuteContract)msg).Sender = WalletAddress;
            }
        }

        var prepareResult = await secretClient.PrepareAndSign(messages, txOptions);
        var request = new SimulateRequest()
        {
            TxBytes = ByteString.CopyFrom(prepareResult)
        };

        var result = await ServiceClient.SimulateAsync(request);
        return result;
    }

    /// <summary>
    /// Broadcasts the specified Tx / message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>SecretTx.</returns>
    public async Task<SecretTx> Broadcast(MsgBase message, TxOptions txOptions = null)
    {
        return await Broadcast(new MsgBase[] { message }, txOptions);
    }

    /// <summary>
    /// Broadcasts the specified Tx / messages.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>SecretTx.</returns>
    public async Task<SecretTx> Broadcast(IMessage message, TxOptions txOptions = null)
    {
        return await Broadcast(new MsgBase[] { new Msg(message) }, txOptions);
    }

    /// <summary>
    /// Broadcasts the specified Tx / message.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>SingleSecretTx&lt;T&gt;.</returns>
    public async Task<SingleSecretTx<T>> Broadcast<T>(MsgBase message, TxOptions txOptions = null)
    {
        return await Broadcast<T>(new MsgBase[] { message }, txOptions);
    }

    /// <summary>
    /// Broadcasts the specified Tx / messages.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message">The message.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>SingleSecretTx&lt;T&gt;.</returns>
    public async Task<SingleSecretTx<T>> Broadcast<T>(IMessage message, TxOptions txOptions = null)
    {
        return await Broadcast<T>(new MsgBase[] { new Msg(message) }, txOptions);
    }

    /// <summary>
    /// Broadcasts the specified Tx / messages.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="messages">The messages.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>SingleSecretTx&lt;T&gt;.</returns>
    public async Task<SingleSecretTx<T>> Broadcast<T>(MsgBase[] messages, TxOptions txOptions = null)
    {
        var result = await Broadcast(messages, txOptions);
        if (result != null)
        {
            return new SingleSecretTx<T>(result);
        }
        return null;
    }

    /// <summary>
    /// Broadcasts the specified Tx / messages.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="messages">The messages.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>SingleSecretTx&lt;T&gt;.</returns>
    public async Task<SingleSecretTx<T>> Broadcast<T>(IMessage[] messages, TxOptions txOptions = null)
    {
        var msgBaseList = new List<MsgBase>();
        foreach(IMessage msg in messages)
        {
            msgBaseList.Add(new Msg(msg));
        }
        return await Broadcast<T>(msgBaseList.ToArray(), txOptions);
    }

    /// <summary>
    /// Broadcasts the specified Tx / messages.
    /// </summary>
    /// <param name="messages">The messages.</param>
    /// <param name="txOptions">The tx options.</param>
    /// <returns>SecretTx.</returns>
    public async Task<SecretTx> Broadcast(MsgBase[] messages, TxOptions txOptions = null)
    {
        txOptions = txOptions != null ? txOptions : new TxOptions();       

        var prepareResult = await secretClient.PrepareAndSign(messages, txOptions);

        return await BroadcastTx(prepareResult, txOptions);        
    }

    private async Task<SecretTx> BroadcastTx(byte[] TxBytes, TxOptions txOptions)
    {
        var start = DateTime.Now;
        var txHash = SecretNET.Crypto.Hashes.SHA256(TxBytes).ToHexString().ToUpper();

        var mode = txOptions.BroadcastMode;
        var waitForCommit = txOptions.WaitForCommit;

        var request = new BroadcastTxRequest()
        {
            TxBytes = ByteString.CopyFrom(TxBytes),
            Mode = mode,
        };

        BroadcastTxResponse broadcastResponse = null;

        if (mode == BroadcastMode.Block)
        {
            waitForCommit = true;
            
            var isBroadcastTimedOut = false;

            try
            {
                broadcastResponse = await ServiceClient.BroadcastTxAsync(request);
            }
            catch (Exception ex)
            {
                if (ex.ToString().ToLower().Contains("timed out waiting for tx to be included in a block", StringComparison.CurrentCultureIgnoreCase))
                {
                    isBroadcastTimedOut = true;
                }
            }

            if (!isBroadcastTimedOut)
            {
                return await _queries.DecodeTxResponse(broadcastResponse?.TxResponse);
            }

        }
        else if (mode == BroadcastMode.Sync)
        {
            broadcastResponse = await ServiceClient.BroadcastTxAsync(request);

            // Check for errors
            if (broadcastResponse.TxResponse.Code > 0)
            {
                // Codespace SDK
                // https://github.com/cosmos/cosmos-sdk/blob/main/types/errors/errors.go
                if (broadcastResponse.TxResponse.Codespace.Equals("sdk", StringComparison.CurrentCultureIgnoreCase))
                {
                    var isEnumParsed = System.Enum.TryParse(broadcastResponse.TxResponse.Code.ToString(), true, out CosmosSdkErrorEnum errorEnum);
                    if (isEnumParsed)
                    {
                        return new SecretTx(new CosmosSdkException(errorEnum, broadcastResponse), txHash);
                    }
                }
                else
                {
                    if (request.Mode == BroadcastMode.Sync)
                    {
                        return new SecretTx(new Exception($"Broadcasting transaction failed with code {broadcastResponse?.TxResponse?.Code}(codespace: {broadcastResponse?.TxResponse?.Codespace}). Log: {broadcastResponse?.TxResponse?.RawLog}"), txHash);
                    }
                }
            }
        }
        else if (mode == BroadcastMode.Async)
        {
            broadcastResponse = await ServiceClient.BroadcastTxAsync(request);
        }
        else
        {
            throw new Exception($"Unknown broadcast mode '{mode}', must be either {BroadcastMode.Block}, {BroadcastMode.Sync} or {BroadcastMode.Async}");
        }

        // no wait
        if (!waitForCommit)
        {
            return new SecretTx(txHash);
        }

        var timeoutMs = txOptions.BroadcastTimeoutMs;
        var checkIntervalMs = txOptions.BroadcastCheckIntervalMs;

        // wait BroadcastCheckIntervalMs before checking the first time on chain
        await Task.Delay(TimeSpan.FromMilliseconds(checkIntervalMs / 2));

        while (true)
        {
            SecretTx txResult = null;
            Func<string, Task> getTx = async (hash) =>
            {
                txResult = await _queries.GetTx(hash);
            };

            await Task.WhenAny(getTx(txHash), Task.Delay(TimeSpan.FromMilliseconds(checkIntervalMs)));

            if (txResult != null)
            {
                return txResult;
            }

            if (start + TimeSpan.FromMilliseconds(timeoutMs) < DateTime.Now)
            {
                throw new Exception($"Transaction ID {txHash} was submitted but was not yet found on the chain. You might want to check later.");
            }
        }
    }

}
