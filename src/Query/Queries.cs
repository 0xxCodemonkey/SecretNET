using Cosmos.Base.Abci.V1Beta1;
using SecretNET.Tx;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace SecretNET.Query;

/// <summary>
/// Provides access to all query types / methods.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class Queries : GprcBase
{
    private Service.ServiceClient _txClient;
    private readonly Regex _errorMessageRegEx = new Regex("; message index: (?<msgIndex>\\d+):(?: dispatch: submessages:)* encrypted: (?<encrypted>.+?): (?:instantiate|execute|query) contract failed", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    // Cosmos
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

    // Secret Network
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
    /// Returns a transaction with a txhash. hash is a 64 character upper-case hex string.
    /// </summary>
    /// <param name="hash">The hash.</param>
    /// <param name="tryToDecrypt">if set to <c>true</c> the client tries to decrypt the tx data (works only if the tx was created in the same session / client instance or if the same CreateClientOptions.EncryptionSeed is used).</param>
    /// <returns>SecretTx.</returns>
    public async Task<SecretTx> GetTx(string hash, bool tryToDecrypt = true)
    {
        var result = await TxsQuery($"tx.hash='{hash.ToUpper()}'", tryToDecrypt);
        return result?[0];
    }


    /// <summary>
    /// Returns all transactions that match a query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="tryToDecrypt">if set to <c>true</c> the client tries to decrypt the tx data (works only if the tx was created in the same session / client instance or if the same CreateClientOptions.EncryptionSeed is used).</param>
    /// <returns>SecretTx[].</returns>
    public async Task<SecretTx[]> TxsQuery(string query, bool tryToDecrypt = false)
    {
        if (!String.IsNullOrWhiteSpace(query))
        {
            GetTxsEventRequest request = new GetTxsEventRequest();
            request.Events.Add(query.Split(" AND ").Select(q => q.Trim()).ToList());

            var result = await GetTxsEvent(request, tryToDecrypt);
            return result;
        }
        return null;
    }

    /// <summary>
    /// Returns all transactions that matches the specified events (GetTxsEventRequest.Events).
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="tryToDecrypt">if set to <c>true</c> the client tries to decrypt the tx data (works only if the tx was created in the same session / client instance or if the same CreateClientOptions.EncryptionSeed is used).</param>
    /// <returns>GetTxsEventResponse.</returns>
    public async Task<SecretTx[]> GetTxsEvent(GetTxsEventRequest request, bool tryToDecrypt = false)
    {
        var result = await ServiceClient.GetTxsEventAsync(request);

        var decodedResponses = await this.DecodeTxResponses(result?.TxResponses?.ToArray(), tryToDecrypt);
        return decodedResponses;
    }

    /// <summary>
    /// Account queries.
    /// </summary>
    public AuthQueryClient Auth
    {
        get { return _authQuery; }
    }

    /// <summary>
    /// Authorization queries (Grants, etc.).
    /// </summary>
    /// <value>The authz.</value>
    public AuthzQueryClient Authz
    {
        get { return _authzQuery; }
    }

    /// <summary>
    /// Bank queries (balance, etc.).
    /// </summary>
    /// <value>The bank.</value>
    public BankQueryClient Bank
    {
        get { return _bankQuery; }
    }
    /// <summary>
    /// Compute queries (QueryContract, GetCodeHash, ContractInfo, etc.).
    /// </summary>
    /// <value>The compute.</value>
    public ComputeQueryClient Compute
    {
        get { return _computeQuery; }
    }

    /// <summary>
    /// Distribution queries.
    /// </summary>
    /// <value>The distribution.</value>
    public DistributionQueryClient Distribution
    {
        get { return _distributionQuery; }
    }

    /// <summary>
    /// Evidence queries.
    /// </summary>
    /// <value>The evidence.</value>
    public EvidenceQueryClient Evidence
    {
        get { return _evidenceQuery; }
    }
    /// <summary>
    /// Feegrant queries.
    /// </summary>
    /// <value>The feegrant.</value>
    public FeegrantQueryClient Feegrant
    {
        get { return _feegrantQuery; }
    }

    /// <summary>
    /// Governance queries.
    /// </summary>
    /// <value>The gov.</value>
    public GovQueryClient Gov
    {
        get { return _govQuery; }
    }

    /// <summary>
    /// IBC Channel queries.
    /// </summary>
    /// <value>The ibc channel.</value>
    public IbcChannelQueryClient IbcChannel
    {
        get { return _ibcChannelQuery; }
    }
    /// <summary>
    /// IBC Client queries.
    /// </summary>
    /// <value>The ibc client.</value>
    public IbcClientQueryClient IbcClient
    {
        get { return _ibcClientQuery; }
    }
    /// <summary>
    /// IBC Connection queries.
    /// </summary>
    /// <value>The ibc connection.</value>
    public IbcConnectionQueryClient IbcConnection
    {
        get { return _ibcConnectionQuery; }
    }

    /// <summary>
    /// IBC Transfer queries.
    /// </summary>
    /// <value>The ibc transfer.</value>
    public IbcTransferQueryClient IbcTransfer
    {
        get { return _ibcTransferQuery; }
    }

    /// <summary>
    /// Mint queries.
    /// </summary>
    /// <value>The mint.</value>
    public MintQueryClient Mint
    {
        get { return _mintQuery; }
    }

    /// <summary>
    /// Params queries.
    /// </summary>
    /// <value>The parameters.</value>
    public ParamsQueryClient Params
    {
        get { return _paramsQuery; }
    }

    /// <summary>
    /// Registration queries (TxKey, etc).
    /// </summary>
    /// <value>The registration.</value>
    public RegistrationQueryClient Registration
    {
        get { return _registrationQuery; }
    }

    /// <summary>
    /// Slashing queries.
    /// </summary>
    /// <value>The slashing.</value>
    public SlashingQueryClient Slashing
    {
        get { return _slashingQuery; }
    }

    /// <summary>
    /// Staking queries.
    /// </summary>
    /// <value>The staking.</value>
    public StakingQueryClient Staking
    {
        get { return _stakingQuery; }
    }

    /// <summary>
    /// Tendermint queries (GetNodeInfo, GetLatestBlock, GetBlockByHeight, etc).
    /// </summary>
    /// <value>The tendermint.</value>
    public TendermintQueryClient Tendermint
    {
        get { return _tendermintQuery; }
    }

    /// <summary>
    /// Upgrade queries.
    /// </summary>
    /// <value>The upgrade.</value>
    public UpgradeQueryClient Upgrade
    {
        get { return _upgradeQuery; }
    }

    // internal

    internal async Task<SecretTx> DecodeTxResponse(TxResponse txResponse, bool tryToDecrypt = false)
    {
        if (txResponse == null) return null;

        var result = await DecodeTxResponses(new TxResponse[] { txResponse }, tryToDecrypt);
        return result?[0];
    }

    internal async Task<SecretTx[]> DecodeTxResponses(TxResponse[] txResponses, bool tryToDecrypt = false)
    {
        if (txResponses == null || txResponses.Length == 0) return null;
        var result = new ConcurrentBag<SecretTx>();

        var encryptionUtils = await GetEncryptionUtils();

        await Parallel.ForEachAsync(txResponses, async (txResponse, cancellationToken) =>
        {
            var decodedTx = txResponse.Tx.Unpack<Cosmos.Tx.V1Beta1.Tx>();

            var nonces = new ComputeMsgToNonce();

            // Decoded input tx messages
            for (var i = 0; i < decodedTx?.Body?.Messages?.Count; i++)
            {
                var rawMsg = decodedTx.Body.Messages[i];

                var messageDecoder = MsgDecoderRegistry.Get(rawMsg.TypeUrl);
                if (messageDecoder == null) continue;

                var msg = messageDecoder(rawMsg);

                if (tryToDecrypt)
                {
                    // Check if the message needs decryption
                    byte[] inputMsgBytes = null;
                    if (rawMsg.TypeUrl.IsProtoType(MsgGrantAuthorization.MsgInstantiateContract))
                    {
                        inputMsgBytes = ((Secret.Compute.V1Beta1.MsgInstantiateContract)msg).InitMsg.Span.ToArray();
                    }
                    else if (rawMsg.TypeUrl.IsProtoType(MsgGrantAuthorization.MsgExecuteContract))
                    {
                        inputMsgBytes = ((Secret.Compute.V1Beta1.MsgExecuteContract)msg).Msg.Span.ToArray();
                    }

                    if (inputMsgBytes != null && inputMsgBytes.Length > 0)
                    {
                        // Encrypted, try to decrypt
                        try
                        {
                            var nonce = new ArraySegment<byte>(inputMsgBytes, 0, 32).ToArray();

                            var accountPubkey = new ArraySegment<byte>(inputMsgBytes, 32, 32).ToArray(); // unused in decryption
                            var ciphertext = new ArraySegment<byte>(inputMsgBytes, 64, inputMsgBytes.Length - 64).ToArray();
                            var encryptionUtils = await GetEncryptionUtils();
                            var plaintext = await encryptionUtils.Decrypt(ciphertext, nonce);

                            if (rawMsg.TypeUrl.IsProtoType(MsgGrantAuthorization.MsgInstantiateContract))
                            {
                                ((Secret.Compute.V1Beta1.MsgInstantiateContract)msg).InitMsg = ByteString.CopyFrom(new ArraySegment<byte>(plaintext, 64, plaintext.Length - 64).ToArray());
                            }
                            else if (rawMsg.TypeUrl.IsProtoType(MsgGrantAuthorization.MsgExecuteContract))
                            {
                                ((Secret.Compute.V1Beta1.MsgExecuteContract)msg).Msg = ByteString.CopyFrom(new ArraySegment<byte>(plaintext, 64, plaintext.Length - 64).ToArray());
                            }

                            nonces[i] = nonce; // Fill nonces array to later use it in output decryption

                        }
                        catch (Exception ex)
                        {
                            // Not encrypted or can't decrypt because not original sender
                        }
                    }
                }

                decodedTx.Body.Messages[i] = Any.Pack(msg);
            }

            txResponse.Tx = Any.Pack(decodedTx);

            var secretTx = new SecretTx(txResponse);

            Func<int, byte[]> getNounce = (msgIndex) =>
            {
                if (msgIndex < nonces.Count())
                {
                    var nonce = nonces[msgIndex];
                    if (nonce != null && nonce.Length == 32)
                    {
                        return nonce;
                    }
                }
                return null;
            };

            var tx = txResponse;
            List<CosmosJsonLog> jsonLogs = null;
            var arrayLog = new List<CosmosArrayLog>();
            var rawLog = (string)tx.RawLog.Clone();

            if (tx.Code == 0 && !string.IsNullOrWhiteSpace(tx.RawLog))
            {
                jsonLogs = JsonConvert.DeserializeObject<List<CosmosJsonLog>>(tx.RawLog);

                for (int i = 0; i < jsonLogs.Count(); i++)
                {
                    var log = jsonLogs[i];
                    if (!log.MsgIndex.HasValue)
                    {
                        log.MsgIndex = i;
                    }

                    foreach (var ev in log.Events)
                    {
                        foreach (var attr in ev.Attributes)
                        {
                            // Try to decrypt
                            if (tryToDecrypt && ev.MessageType.Equals("wasm", StringComparison.CurrentCultureIgnoreCase))
                            {
                                var nonce = getNounce(log.MsgIndex.GetValueOrDefault());
                                if (nonce != null)
                                {
                                    try
                                    {
                                        if (attr.Key.IsBase64String())
                                        {
                                            attr.Key = Encoding.UTF8.GetString(await encryptionUtils.Decrypt(Convert.FromBase64String(attr.Key), nonce));
                                        }
                                    }
                                    catch { }
                                    try
                                    {
                                        if (attr.Value.IsBase64String())
                                        {
                                            attr.Value = Encoding.UTF8.GetString(await encryptionUtils.Decrypt(Convert.FromBase64String(attr.Value), nonce));
                                        }
                                    }
                                    catch { }
                                }
                            }

                            arrayLog.Add(new CosmosArrayLog()
                            {
                                MsgIndex = log.MsgIndex.GetValueOrDefault(),
                                EventType = ev.MessageType,
                                Key = attr.Key,
                                Value = attr.Value,
                            });
                        }
                    }
                }
            }
            else if (tx.Code != 0 && !string.IsNullOrWhiteSpace(tx.RawLog))
            {
                try
                {
                    var errorMatches = _errorMessageRegEx.Match(tx.RawLog);
                    if (errorMatches.Success &&
                        !string.IsNullOrWhiteSpace(errorMatches.Groups["msgIndex"]?.Value) &&
                        !string.IsNullOrWhiteSpace(errorMatches.Groups["encrypted"]?.Value))
                    {
                        var encryptedError = Convert.FromBase64String(errorMatches.Groups["encrypted"].Value);
                        var msgIndex = Convert.ToInt32(errorMatches.Groups["msgIndex"].Value);
                        var nonce = getNounce(msgIndex);
                        if (nonce != null)
                        {
                            var decryptedBase64Error = Encoding.UTF8.GetString(await encryptionUtils.Decrypt(encryptedError, nonce));

                            rawLog = rawLog.Replace($"encrypted: {errorMatches.Groups["encrypted"].Value}", decryptedBase64Error);
                        }
                    }

                    // Check for errors
                    // Cosmos SDK Errors
                    // https://github.com/cosmos/cosmos-sdk/blob/main/types/errors/errors.go
                    if (tx.Codespace.Equals("sdk", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var isEnumParsed = System.Enum.TryParse(tx.Code.ToString(), true, out CosmosSdkErrorEnum errorEnum);
                        if (isEnumParsed)
                        {
                            secretTx.Exceptions.Add(new CosmosSdkException(errorEnum, txResponse));
                            //throw new CosmosSdkException(errorEnum, encryptedResult);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Not encrypted or can't decrypt because not original sender
                    secretTx.Exceptions.Add(new Exception("Not encrypted or can't decrypt because not original sender"));
                    secretTx.Exceptions.Add(ex);
                }
            }

            var txMsgData = TxMsgData.Parser.ParseFrom(Convert.FromHexString(tx.Data));
            var data = new byte[txMsgData.Data.Count][];
            for (int msgIndex = 0; msgIndex < txMsgData.Data.Count; msgIndex++)
            {
                data[msgIndex] = txMsgData.Data[msgIndex].Data.ToByteArray();
                if (data[msgIndex]?.Length > 0)
                {
                    var nonce = getNounce(msgIndex);
                    if (nonce != null && tryToDecrypt)
                    {
                        var rawMsg = decodedTx?.Body?.Messages[msgIndex];
                        var messageDecoder = MsgDecoderRegistry.Get(rawMsg.TypeUrl);
                        if (messageDecoder == null) continue;

                        var msg = messageDecoder(rawMsg);

                        if (rawMsg.TypeUrl.IsProtoType(MsgGrantAuthorization.MsgInstantiateContract))
                        {
                            var decoded = Secret.Compute.V1Beta1.MsgInstantiateContractResponse.Parser.ParseFrom(txMsgData.Data[msgIndex].Data);
                            var decrypted = Convert.FromBase64String(Encoding.UTF8.GetString(await encryptionUtils.Decrypt(decoded.Data.ToByteArray(), nonce)));
                            var decryptedMsg = new Secret.Compute.V1Beta1.MsgInstantiateContractResponse()
                            {
                                Address = decoded.Address,
                                Data = ByteString.CopyFrom(decrypted)
                            };
                            data[msgIndex] = decryptedMsg.Encode();
                        }
                        else if (rawMsg.TypeUrl.IsProtoType(MsgGrantAuthorization.MsgExecuteContract))
                        {
                            var decoded = Secret.Compute.V1Beta1.MsgExecuteContractResponse.Parser.ParseFrom(txMsgData.Data[msgIndex].Data);
                            var decrypted = Convert.FromBase64String(Encoding.UTF8.GetString(await encryptionUtils.Decrypt(decoded.Data.ToByteArray(), nonce)));
                            var jsonData = Encoding.UTF8.GetString(decrypted);
                            var decryptedMsg = new Secret.Compute.V1Beta1.MsgExecuteContractResponse()
                            {
                                Data = ByteString.CopyFrom(decrypted)
                            };
                            data[msgIndex] = decryptedMsg.Encode();
                        }
                    }
                    else
                    {
                        data[msgIndex] = txMsgData.Data[msgIndex].Data.ToByteArray();
                    }
                }
            }

            secretTx.RawLog = rawLog;
            secretTx.JsonLog = jsonLogs;
            secretTx.ArrayLog = arrayLog;
            secretTx.Data = data;
            secretTx.Success = true;

            result.Add(secretTx);

        });

        if (!result.IsEmpty)
        {
            return result.ToArray();
        }

        return null;
    }


}
