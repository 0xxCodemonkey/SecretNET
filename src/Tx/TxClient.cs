using Cosmos.Tx.V1Beta1;
using Cosmos.Base.Abci.V1Beta1;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Google.Protobuf;

namespace SecretNET.Tx;

public class TxClient : GprcBase
{
    private Service.ServiceClient _txClient;
    
    private ComputeTx _computeTx;

    internal TxClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {        
        _computeTx = new ComputeTx(this);
    }

    private Service.ServiceClient client
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

    public ComputeTx Compute
    {
        get { return _computeTx; }
    }

    public async Task<SimulateResponse> Simulate(MsgBase message, TxOptions txOptions = null)
    {
        if (message is MsgExecuteContract && string.IsNullOrEmpty(((MsgExecuteContract)message).Sender))
        {
            ((MsgExecuteContract)message).Sender = WalletAddress;
        }
        return await Simulate(new MsgBase[] { message }, txOptions);
    }

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

        var result = await client.SimulateAsync(request);
        return result;
    }

    public async Task<SecretTx> Broadcast(MsgBase message, TxOptions txOptions = null)
    {
        return await Broadcast(new MsgBase[] { message }, txOptions);
    }

    public async Task<SingleSecretTx<T>> Broadcast<T>(MsgBase message, TxOptions txOptions = null)
    {
        return await Broadcast<T>(new MsgBase[] { message }, txOptions);
    }

    public async Task<SingleSecretTx<T>> Broadcast<T>(MsgBase[] messages, TxOptions txOptions = null)
    {
        var result = await Broadcast(messages, txOptions);
        if (result != null)
        {
            return new SingleSecretTx<T>(result);
        }
        return null;
    }

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

        Console.WriteLine("BroadcastTx Calculated TxHash: " + txHash);

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
                broadcastResponse = await client.BroadcastTxAsync(request);
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
                return await DecodeTxResponse(broadcastResponse?.TxResponse);
            }

        }
        else if (mode == BroadcastMode.Sync)
        {
            broadcastResponse = await client.BroadcastTxAsync(request);

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
            broadcastResponse = await client.BroadcastTxAsync(request);
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
                txResult = await GetTx(hash);
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

    private readonly Regex _errorMessageRegEx = new Regex("; message index: (?<msgIndex>\\d+):(?: dispatch: submessages:)* encrypted: (?<encrypted>.+?): (?:instantiate|execute|query) contract failed", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    /// <summary>
    /// Gets the tx.
    /// </summary>
    /// <param name="hash">The hash.</param>
    /// <param name="tryToDecrypt">if set to <c>true</c> [try to decrypt].</param>
    /// <returns>SecretTx.</returns>
    public async Task<SecretTx> GetTx(string hash, bool tryToDecrypt = true)
    {
        var result = await TxsQuery($"tx.hash='{hash}'", tryToDecrypt);
        return result?[0];
    }


    /// <summary>
    /// TXSs the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="tryToDecrypt">if set to <c>true</c> [try to decrypt].</param>
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
    /// GetTxsEvent fetches txs by event.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="tryToDecrypt">if set to <c>true</c> [try to decrypt].</param>
    /// <returns>GetTxsEventResponse.</returns>
    public async Task<SecretTx[]> GetTxsEvent(GetTxsEventRequest request, bool tryToDecrypt = false)
    {
        var result = await client.GetTxsEventAsync(request);

        var decodedResponses = await this.DecodeTxResponses(result?.TxResponses?.ToArray(), tryToDecrypt);
        return decodedResponses;
    }


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
                            //Console.WriteLine("DecodeTxResponses nonce: " + nonce.ToHexString());

                            var accountPubkey = new ArraySegment<byte>(inputMsgBytes, 32, 32).ToArray(); // unused in decryption
                            var ciphertext = new ArraySegment<byte>(inputMsgBytes, 64, inputMsgBytes.Length - 64).ToArray();
                            var plaintext = await Encryption.Decrypt(ciphertext, nonce);

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

            if (tx.Code == 0 && !string.IsNullOrEmpty(tx.RawLog))
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
                                            attr.Key = Encoding.UTF8.GetString(await Encryption.Decrypt(Convert.FromBase64String(attr.Key), nonce));
                                        }
                                    }
                                    catch { }
                                    try
                                    {
                                        if (attr.Value.IsBase64String())
                                        {
                                            attr.Value = Encoding.UTF8.GetString(await Encryption.Decrypt(Convert.FromBase64String(attr.Value), nonce));
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
            else if (tx.Code != 0 && !string.IsNullOrEmpty(tx.RawLog))
            {
                try
                {
                    var errorMatches = _errorMessageRegEx.Match(tx.RawLog);
                    if (errorMatches.Success &&
                        !string.IsNullOrEmpty(errorMatches.Groups["msgIndex"]?.Value) &&
                        !string.IsNullOrEmpty(errorMatches.Groups["encrypted"]?.Value))
                    {
                        var encryptedError = Convert.FromBase64String(errorMatches.Groups["encrypted"].Value);
                        var msgIndex = Convert.ToInt32(errorMatches.Groups["msgIndex"].Value);
                        var nonce = getNounce(msgIndex);
                        if (nonce != null)
                        {
                            var decryptedBase64Error = Encoding.UTF8.GetString(await Encryption.Decrypt(encryptedError, nonce));

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
                            var decrypted = Convert.FromBase64String(Encoding.UTF8.GetString(await Encryption.Decrypt(decoded.Data.ToByteArray(), nonce)));
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
                            var decrypted = Convert.FromBase64String(Encoding.UTF8.GetString(await Encryption.Decrypt(decoded.Data.ToByteArray(), nonce)));
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
