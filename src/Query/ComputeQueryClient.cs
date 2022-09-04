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
    /// <param name="contractAddress"></param>
    /// <param name="queryMsg"></param>
    /// <param name="codeHash">codehash is optional but makes the first call way faster (after that the hash is cached in the client)</param>
    /// <returns></returns>
    public async Task<SecretQueryContractResult<R>> QueryContract<R>(string contractAddress, object queryMsg, string codeHash = null) where R : class
    {
        if (string.IsNullOrEmpty(codeHash))
        {
            codeHash = await GetCodeHash(contractAddress);
        }

        SecretQueryContractResult<R> result = null;

        if (!string.IsNullOrEmpty(codeHash))
        {
            var queryMsgString = queryMsg is string ? queryMsg : JsonConvert.SerializeObject(queryMsg);
            var encryptedQuery = await Encryption.Encrypt(codeHash, queryMsgString);
            if (encryptedQuery != null && encryptedQuery.Length > 32)
            {
                var nonce = new ArraySegment<byte>(encryptedQuery, 0, 32).ToArray();
                try
                {
                    var request = new Secret.Compute.V1Beta1.QuerySmartContractStateRequest()
                    {
                        Address = AddressToBytes(contractAddress).GetByteStringFromBase64(),
                        QueryData = encryptedQuery.GetByteStringFromBase64()
                    };
                    var queryResponse = await client.SmartContractStateAsync(request);

                    if (queryResponse != null)
                    {
                        var decryptedBase64Result = await Encryption.Decrypt(queryResponse.Data.ToByteArray(), nonce);
                        var utf8Str = Encoding.UTF8.GetString(decryptedBase64Result);
                        var resultBytes = Convert.FromBase64String(utf8Str);
                        var resultString = Encoding.UTF8.GetString(resultBytes);

                        R queryResult = JsonConvert.DeserializeObject<R>(resultString);
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
                        !string.IsNullOrEmpty(errorMatch.Groups["error"]?.Value) &&
                        !string.IsNullOrEmpty(errorMatch.Groups["encrypted"]?.Value))
                        {
                            var encryptedError = Convert.FromBase64String(errorMatch.Groups["encrypted"].Value);
                            var error = errorMatch.Groups["error"].Value;

                            var decryptedBase64Error = Encoding.UTF8.GetString(await Encryption.Decrypt(encryptedError, nonce));

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
    /// <param name="contractAddress"></param>
    /// <returns></returns>
    public async Task<string> GetCodeHash(string contractAddress)
    {
        // Cache => via Provider Hashes je chainId speichern
        if (_codeHashCache.ContainsKey(contractAddress))
        {
            return _codeHashCache[contractAddress];
        }

        string codeHash = null;
        var codeInfoResult = await ContractInfo(contractAddress);
        if (codeInfoResult != null && codeInfoResult.CodeId > 0)
        {
            var code = await Code(codeInfoResult.CodeId);
            if (code != null && !String.IsNullOrEmpty(code.CodeHash))
            {
                if (!_codeHashCache.ContainsKey(contractAddress))
                {
                    _codeHashCache.TryAdd(contractAddress, code.CodeHash);
                }
                return code.CodeHash;
            }
        }

        return null;
    }

    /// <summary>
    /// Get codeHash from a code id
    /// </summary>
    /// <param name="codeId"></param>
    /// <returns></returns>
    public async Task<string> GetCodeHashByCodeId(ulong codeId)
    {
        // Cache => via Provider Hashes je chainId speichern
        if (_codeHashCacheByCodeId.ContainsKey(codeId))
        {
            return _codeHashCacheByCodeId[codeId];
        }

        string codeHash = null;
        var code = await Code(codeId);
        if (code != null && !String.IsNullOrEmpty(code.CodeHash))
        {
            if (!_codeHashCacheByCodeId.ContainsKey(codeId))
            {
                _codeHashCacheByCodeId.TryAdd(codeId, code.CodeHash);
            }
            return code.CodeHash;
        }

        return null;
    }

    /// <summary>
    /// Get WASM bytecode and metadata for a code id
    /// </summary>
    /// <param name="codeId"></param>
    /// <returns></returns>
    public async Task<SecretCodeInfo> Code(ulong codeId)
    {
        var request = new Secret.Compute.V1Beta1.QueryCodeRequest()
        {
            CodeId = codeId
        };
        var result = await client.CodeAsync(request);
        if (result?.CodeInfo != null)
        {
            var parsedInfo = ParseCodeInfo(result.CodeInfo);
            return parsedInfo;
        }
        return null;
    }

    /// <summary>
    /// Get metadata of a Secret Contract
    /// </summary>
    /// <param name="contractAddress"></param>
    /// <returns></returns>
    public async Task<SecretContractInfo> ContractInfo(string contractAddress)
    {
        var request = new Secret.Compute.V1Beta1.QueryContractInfoRequest()
        {
            Address = ByteString.FromBase64(Convert.ToBase64String(AddressToBytes(contractAddress)))
        };
        var result = await client.ContractInfoAsync(request);
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
    /// <param name="codeId"></param>
    /// <returns></returns>
    public async Task<List<SecretContractInfo>> ContractsByCode(ulong codeId)
    {
        var request = new Secret.Compute.V1Beta1.QueryContractsByCodeRequest()
        {
            CodeId = codeId
        };
        var result = await client.ContractsByCodeAsync(request);
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
    /// <returns></returns>
    public async Task<List<SecretCodeInfo>> Codes()
    {
        var result = await client.CodesAsync(new Empty());

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
            parsedInfo.Address = contractInfo.Address.ToStringUtf8();
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
            return new SecretCodeInfo()
            {
                CodeId = codeInfo.CodeId,
                CreatorAddress = SecretNetworkClient.BytesToAddress(codeInfo.Creator.ToByteArray()),
                CodeHash = Convert.ToHexString(codeInfo.DataHash.ToByteArray()).ToLower(),
                Builder = codeInfo.Builder,
                Source = codeInfo.Source
            };
        }
        return null;
    }
}

