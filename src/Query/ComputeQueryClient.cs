using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace SecretNET.Query;

/// <summary>
/// Class ComputeQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class ComputeQueryClient : GprcBase
{
    private readonly Regex _errorMessageRegEx = new Regex("encrypted: (?<encrypted>.+?): (?<error>(?:instantiate|execute|query) contract failed)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
    private ConcurrentDictionary<string, string> _codeHashCache = new ConcurrentDictionary<string, string>();
    private ConcurrentDictionary<ulong, string> _codeHashCacheByCodeId = new ConcurrentDictionary<ulong, string>();

    private Secret.Compute.V1Beta1.Query.QueryClient? _queryClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComputeQueryClient"/> class.
    /// </summary>
    /// <param name="secretNetworkClient">The secret network client.</param>
    internal ComputeQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Secret.Compute.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Secret.Compute.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Secret.Compute.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Query a Secret Contract
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="contractAddress">The contract address.</param>
    /// <param name="queryMsg">The query MSG.</param>
    /// <param name="codeHash">codehash is optional but makes the first call way faster (after that the hash is cached in the client)</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>SecretQueryContractResult&lt;R&gt;.</returns>
    public async Task<SecretQueryContractResult<R>> QueryContract<R>(string contractAddress, object queryMsg, string codeHash = null, Metadata metadata = null) where R : class
    {
        if (string.IsNullOrWhiteSpace(codeHash))
        {
            codeHash = await GetCodeHash(contractAddress, metadata);
        }

        SecretQueryContractResult<R> result = null;
        var encryptionUtils = await GetEncryptionUtils();

        if (!string.IsNullOrWhiteSpace(codeHash))
        {
            var queryMsgString = queryMsg is string ? queryMsg : JsonConvert.SerializeObject(queryMsg);
            var encryptedQuery = await encryptionUtils.Encrypt(codeHash, queryMsgString);
            if (encryptedQuery != null && encryptedQuery.Length > 32)
            {
                var nonce = new ArraySegment<byte>(encryptedQuery, 0, 32).ToArray();
                try
                {
                    var request = new Secret.Compute.V1Beta1.QuerySecretContractRequest()
                    {
                        ContractAddress = contractAddress,
                        Query = encryptedQuery.GetByteStringFromBase64()
                    };
                    var queryResponse = await client.QuerySecretContractAsync(request, metadata);

                    if (queryResponse != null)
                    {
                        var decryptedBase64Result = await encryptionUtils.Decrypt(queryResponse.Data.ToByteArray(), nonce);
                        var utf8Str = Encoding.UTF8.GetString(decryptedBase64Result);
                        var resultBytes = Convert.FromBase64String(utf8Str);
                        var resultString = Encoding.UTF8.GetString(resultBytes);
                        R queryResult = null;
                        if (typeof(R) == typeof(string))
                        {
                            queryResult = resultString as R;
                        }
                        else
                        {
                            queryResult = JsonConvert.DeserializeObject<R>(resultString);
                        }
                        result = new SecretQueryContractResult<R>(queryResult);
                        result.RawResponse = resultString;
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        var errorMatch = _errorMessageRegEx.Match(ex.Message);
                        if (errorMatch.Success &&
                        !string.IsNullOrWhiteSpace(errorMatch.Groups["error"]?.Value) &&
                        !string.IsNullOrWhiteSpace(errorMatch.Groups["encrypted"]?.Value))
                        {
                            var encryptedError = Convert.FromBase64String(errorMatch.Groups["encrypted"].Value);
                            var error = errorMatch.Groups["error"].Value;

                            var decryptedBase64Error = Encoding.UTF8.GetString(await encryptionUtils.Decrypt(encryptedError, nonce));

                            try
                            {
                                JObject errorObject = null;
                                if (decryptedBase64Error.IsBase64String())
                                {
                                    errorObject = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(decryptedBase64Error)));
                                }
                                else
                                {
                                    errorObject = JObject.Parse(decryptedBase64Error);
                                }
                                result = new SecretQueryContractResult<R>(new SmartContractException(error, errorObject, decryptedBase64Error, ex));
                            }
                            catch (Exception parseError)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            result = new SecretQueryContractResult<R>(ex);
                        }
                    }
                    catch (Exception decryptionError)
                    {
                        throw ex;
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Get codeHash of a Secret Contract
    /// </summary>
    /// <param name="contractAddress">The contract address.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>System.String.</returns>
    public async Task<string> GetCodeHash(string contractAddress, Metadata metadata = null)
    {
        // Cache => via Provider Hashes je chainId speichern
        if (_codeHashCache.ContainsKey(contractAddress))
        {
            return _codeHashCache[contractAddress];
        }

        var codeInfoResult = await ContractInfo(contractAddress, metadata);
        if (codeInfoResult != null && codeInfoResult.CodeId > 0)
        {
            var code = await Code(codeInfoResult.CodeId, metadata);
            if (code.CodeInfo != null && !string.IsNullOrWhiteSpace(code.CodeInfo?.CodeHash))
            {
                if (!_codeHashCache.ContainsKey(contractAddress))
                {
                    _codeHashCache.TryAdd(contractAddress, code.CodeInfo?.CodeHash);
                }
                return code.CodeInfo?.CodeHash;
            }
        }

        return null;
    }

    /// <summary>
    /// Get codeHash from a code id
    /// </summary>
    /// <param name="codeId">The code identifier.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>System.String.</returns>
    public async Task<string> GetCodeHashByCodeId(ulong codeId, Metadata metadata = null)
    {
        // Cache => via Provider Hashes je chainId speichern
        if (_codeHashCacheByCodeId.ContainsKey(codeId))
        {
            return _codeHashCacheByCodeId[codeId];
        }

        var code = await Code(codeId, metadata);
        if (code.CodeInfo != null && !string.IsNullOrWhiteSpace(code.CodeInfo.CodeHash))
        {
            if (!_codeHashCacheByCodeId.ContainsKey(codeId))
            {
                _codeHashCacheByCodeId.TryAdd(codeId, code.CodeInfo.CodeHash);
            }
            return code.CodeInfo.CodeHash;
        }

        return null;
    }

    /// <summary>
    /// Get WASM bytecode and metadata for a code id
    /// </summary>
    /// <param name="codeId">The code identifier.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>SecretCodeInfo.</returns>
    public async Task<(SecretCodeInfo CodeInfo, byte[] WasmBytes)> Code(ulong codeId, Metadata metadata = null)
    {
        var request = new Secret.Compute.V1Beta1.QueryByCodeIDRequest
        {
            CodeId = codeId
        };
        var result = await client.CodeAsync(request, metadata);
        if (result?.CodeInfo != null)
        {
            var parsedInfo = ParseCodeInfo(result.CodeInfo);
            return (parsedInfo, result.Wasm.ToByteArray());
        }
        return (null, null);
    }

    /// <summary>
    /// Get metadata of a Secret Contract
    /// </summary>
    /// <param name="contractAddress">The contract address.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>SecretContractInfo.</returns>
    public async Task<SecretContractInfo> ContractInfo(string contractAddress, Metadata metadata = null)
    {
        var request = new Secret.Compute.V1Beta1.QueryByContractAddressRequest
        {
            ContractAddress = contractAddress
        };
        var result = await client.ContractInfoAsync(request, metadata);
        if (result?.ContractInfo != null)
        {
            var parsedInfo = ParseContractInfo(result.ContractInfo);
            parsedInfo.Address = contractAddress;
            return parsedInfo;
        }
        return null;
    }

    /// <summary>
    /// Get all contracts that were instantiated from a code id.
    /// </summary>
    /// <param name="codeId">The code identifier.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>List&lt;SecretContractInfo&gt;.</returns>
    public async Task<List<SecretContractInfo>> ContractsByCode(ulong codeId, Metadata metadata = null)
    {
        var request = new Secret.Compute.V1Beta1.QueryByCodeIDRequest
        {
            CodeId = codeId
        };
        var result = await client.ContractsByCodeIDAsync(request, metadata);
        if ((result?.ContractInfos?.Any()).GetValueOrDefault())
        {
            var resultList = new List<SecretContractInfo>();
            foreach (var contractInfo in result.ContractInfos)
            {
                var parsedInfo = ParseContractInfo(contractInfo);
                if (parsedInfo != null)
                {
                    resultList.Add(parsedInfo);
                }
            }
            return resultList;
        }
        return null;
    }

    /// <summary>
    /// Query all codes on chain.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    /// <returns>List&lt;SecretCodeInfo&gt;.</returns>
    public async Task<List<SecretCodeInfo>> Codes(Metadata metadata = null)
    {
        var result = await client.CodesAsync(new Empty(), metadata);

        if ((result?.CodeInfos?.Any()).GetValueOrDefault())
        {
            var resultList = new List<SecretCodeInfo>();
            foreach (var codeInfo in result.CodeInfos)
            {
                var parsedInfo = ParseCodeInfo(codeInfo);
                if (parsedInfo != null)
                {
                    resultList.Add(parsedInfo);
                }
            }
            return resultList;
        }

        return null;
    }

    // private

    /// <summary>
    /// Parses the contract information.
    /// </summary>
    /// <param name="contractInfo">The contract information.</param>
    /// <returns>SecretContractInfo.</returns>
    private SecretContractInfo ParseContractInfo(Secret.Compute.V1Beta1.ContractInfo contractInfo)
    {
        if (contractInfo != null)
        {
            var result =  new SecretContractInfo()
            {
                CodeId = contractInfo.CodeId,
                CreatorAddress = SecretNetworkClient.BytesToAddress(contractInfo.Creator.ToByteArray()),
                Label = contractInfo.Label,
                IbcPortId = contractInfo.IbcPortId                
            };
            if (contractInfo.Created != null)
            {
                result.Created = new AbsoluteTxPosition()
                {
                    BlockHeight = contractInfo.Created.BlockHeight,
                    TxIndex = contractInfo.Created.TxIndex
                };
            }
            return result;
        }
        return null;
    }

    /// <summary>
    /// Parses the contract information.
    /// </summary>
    /// <param name="contractInfo">The contract information.</param>
    /// <returns>SecretContractInfo.</returns>
    private SecretContractInfo ParseContractInfo(Secret.Compute.V1Beta1.ContractInfoWithAddress contractInfo)
    {
        if (contractInfo?.ContractInfo != null)
        {
            var parsedInfo = ParseContractInfo(contractInfo.ContractInfo);
            parsedInfo.Address = contractInfo.ContractAddress;
            return parsedInfo;
        }
        return null;
    }

    /// <summary>
    /// Parses the code information.
    /// </summary>
    /// <param name="codeInfo">The code information.</param>
    /// <returns>SecretCodeInfo.</returns>
    private SecretCodeInfo ParseCodeInfo(Secret.Compute.V1Beta1.CodeInfoResponse codeInfo)
    {
        if (codeInfo != null)
        {
            if (!_codeHashCacheByCodeId.ContainsKey(codeInfo.CodeId))
            {
                _codeHashCacheByCodeId.TryAdd(codeInfo.CodeId, codeInfo.CodeHash);
            }

            return new SecretCodeInfo()
            {
                CodeId = codeInfo.CodeId,
                CreatorAddress = codeInfo.Creator,
                CodeHash = codeInfo.CodeHash,
                Builder = codeInfo.Builder,
                Source = codeInfo.Source
            };
        }
        return null;
    }
}

