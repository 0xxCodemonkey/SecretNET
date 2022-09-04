using Cosmos.Base.Abci.V1Beta1;
using System.Text.RegularExpressions;

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
            TxBytes = ByteString.CopyFrom(prepareResult.TxBytes)
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

        var start = DateTime.Now;

        var prepareResult = await secretClient.PrepareAndSign(messages, txOptions);
        var request = new BroadcastTxRequest()
        {
            TxBytes = ByteString.CopyFrom(prepareResult.TxBytes),
            Mode = txOptions?.BroadcastMode ?? BroadcastMode.Sync
        };

        var result = await client.BroadcastTxAsync(request);
        var txHash = result?.TxResponse?.Txhash;

        // Check for errors
        if (result.TxResponse.Code > 0)
        {
            // Codespace SDK
            // https://github.com/cosmos/cosmos-sdk/blob/main/types/errors/errors.go
            if (result.TxResponse.Codespace.Equals("sdk", StringComparison.CurrentCultureIgnoreCase))
            {
                var isEnumParsed = System.Enum.TryParse(result.TxResponse.Code.ToString(), true, out CosmosSdkErrorEnum errorEnum);
                if (isEnumParsed)
                {
                    return new SecretTx(new CosmosSdkException(errorEnum, result), txHash);
                }
            }
            else
            {
                if (request.Mode == BroadcastMode.Sync)
                {
                    return new SecretTx(new Exception($"Broadcasting transaction failed with code {result?.TxResponse?.Code}(codespace: {result?.TxResponse?.Codespace}). Log: {result?.TxResponse?.RawLog}"), txHash);
                }
            }
        }

        // no wait
        if (!txOptions.WaitForCommit)
        {
            return new SecretTx(txHash);
        }

        // wait BroadcastCheckIntervalMs before checking the first time on chain
        await Task.Delay(TimeSpan.FromMilliseconds(txOptions.BroadcastCheckIntervalMs));

        while (true)
        {
            SecretTx txResult = null;
            Func<string, ComputeMsgToNonce, Task> getTx = async (hash, nonces) =>
            {
                txResult = await GetTx(hash, nonces);
            };

            await Task.WhenAny(
                getTx(txHash, prepareResult.Nonces),
                Task.Delay(TimeSpan.FromMilliseconds(txOptions.BroadcastCheckIntervalMs)));

            if (txResult != null)
            {
                return txResult;
            }

            if (start + TimeSpan.FromMilliseconds(txOptions.BroadcastTimeoutMs) < DateTime.Now)
            {
                throw new Exception($"Transaction ID {txHash} was submitted but was not yet found on the chain. You might want to check later.");
            }
        }
    }

    private readonly Regex _errorMessageRegEx = new Regex("; message index: (?<msgIndex>\\d+): encrypted: (?<encrypted>.+?): (?:instantiate|execute|query) contract failed", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    public async Task<SecretTx> GetTx(string hash, ComputeMsgToNonce nonces = null)
    {
        var request = new GetTxRequest()
        {
            Hash = hash
        };

        GetTxResponse encryptedResult = null;
        try
        {
            encryptedResult = await client.GetTxAsync(request);
        }
        catch (RpcException rpcEx)
        {
            if (rpcEx.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
            else
            {
                throw rpcEx;
            }
        }

        if (encryptedResult == null)
        {
            return null;
        }

        var secretTx = new SecretTx(encryptedResult);

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

        // decode result
        if (encryptedResult != null && encryptedResult.TxResponse != null && nonces != null)
        {
            var tx = encryptedResult.TxResponse;
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
                            if (ev.MessageType.Equals("wasm", StringComparison.CurrentCultureIgnoreCase))
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
                            secretTx.Exceptions.Add(new CosmosSdkException(errorEnum, encryptedResult));
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
                var msgData = txMsgData.Data[msgIndex].Data.ToByteArray();
                if (msgData.Length > 0)
                {
                    var nonce = getNounce(msgIndex);
                    if (nonce != null)
                    {
                        try
                        {
                            var decryptedDataBase64 = Encoding.UTF8.GetString(await Encryption.Decrypt(txMsgData.Data[msgIndex].Data.ToByteArray(), nonce));
                            data[msgIndex] = Convert.FromBase64String(decryptedDataBase64);
                        }
                        catch (Exception ex)
                        {
                            // Not encrypted or can't decrypt because not original sender
                            data[msgIndex] = txMsgData.Data[msgIndex].Data.ToByteArray();
                        }
                    }
                }
            }

            secretTx.RawLog = rawLog;
            secretTx.JsonLog = jsonLogs;
            secretTx.ArrayLog = arrayLog;
            secretTx.Data = data;
            secretTx.Success = true;
        }

        return secretTx;
    }

}
