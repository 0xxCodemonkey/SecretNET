using Cosmos.Base.Tendermint.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class TendermintQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class TendermintQueryClient : GprcBase
{
    private Cosmos.Base.Tendermint.V1Beta1.Service.ServiceClient? _queryClient;

    internal TendermintQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Base.Tendermint.V1Beta1.Service.ServiceClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Base.Tendermint.V1Beta1.Service.ServiceClient(grpcMessageInterceptor) : 
                    new Cosmos.Base.Tendermint.V1Beta1.Service.ServiceClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// GetNodeInfo queries the current node info.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>GetNodeInfoResponse.</returns>
    public async Task<GetNodeInfoResponse> GetNodeInfo(GetNodeInfoRequest request)
    {
        var result = await client.GetNodeInfoAsync(request);
        return result;
    }

    /// <summary>
    /// GetSyncing queries node syncing.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>GetSyncingResponse.</returns>
    public async Task<GetSyncingResponse> GetSyncing(GetSyncingRequest request)
    {
        var result = await client.GetSyncingAsync(request);
        return result;
    }

    /// <summary>
    /// GetLatestBlock returns the latest block.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>GetLatestBlockResponse.</returns>
    public async Task<GetLatestBlockResponse> GetLatestBlock(GetLatestBlockRequest request)
    {
        var result = await client.GetLatestBlockAsync(request);
        return result;
    }

    /// <summary>
    /// GetBlockByHeight queries block for given height.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>GetBlockByHeightResponse.</returns>
    public async Task<GetBlockByHeightResponse> GetBlockByHeight(GetBlockByHeightRequest request)
    {
        var result = await client.GetBlockByHeightAsync(request);
        return result;
    }

    /// <summary>
    /// GetLatestValidatorSet queries latest validator-set.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>GetLatestValidatorSetResponse.</returns>
    public async Task<GetLatestValidatorSetResponse> GetLatestValidatorSet(GetLatestValidatorSetRequest request)
    {
        var result = await client.GetLatestValidatorSetAsync(request);
        return result;
    }

    /// <summary>
    /// GetValidatorSetByHeight queries validator-set at a given height.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>GetValidatorSetByHeightResponse.</returns>
    public async Task<GetValidatorSetByHeightResponse> GetValidatorSetByHeight(GetValidatorSetByHeightRequest request)
    {
        var result = await client.GetValidatorSetByHeightAsync(request);
        return result;
    }
}
