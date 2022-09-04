using Ibc.Core.Client.V1;

namespace SecretNET.Query;

/// <summary>
/// Class IbcClientQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class IbcClientQueryClient : GprcBase
{
    private Ibc.Core.Client.V1.Query.QueryClient? _queryClient;

    internal IbcClientQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Ibc.Core.Client.V1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Ibc.Core.Client.V1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Ibc.Core.Client.V1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// ClientState queries an IBC light client.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryClientStateResponse.</returns>
    public async Task<QueryClientStateResponse> ClientState(QueryClientStateRequest request)
    {
        var result = await client.ClientStateAsync(request);
        return result;
    }

    /// <summary>
    /// ClientStates queries all the IBC light clients of a chain.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryClientStatesResponse.</returns>
    public async Task<QueryClientStatesResponse> ClientStates(QueryClientStatesRequest request)
    {
        var result = await client.ClientStatesAsync(request);
        return result;
    }

    /// <summary>
    /// ConsensusState queries a consensus state associated with a client state at a given height.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryConsensusStateResponse.</returns>
    public async Task<QueryConsensusStateResponse> ConsensusState(QueryConsensusStateRequest request)
    {
        var result = await client.ConsensusStateAsync(request);
        return result;
    }

    /// <summary>
    /// ConsensusStates queries all the consensus state associated with a given client.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryConsensusStatesResponse.</returns>
    public async Task<QueryConsensusStatesResponse> ConsensusStates(QueryConsensusStatesRequest request)
    {
        var result = await client.ConsensusStatesAsync(request);
        return result;
    }

    /// <summary>
    /// Status queries the status of an IBC client.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryClientStatusResponse.</returns>
    public async Task<QueryClientStatusResponse> ClientStatus(QueryClientStatusRequest request)
    {
        var result = await client.ClientStatusAsync(request);
        return result;
    }

    /// <summary>
    /// ClientParams queries all parameters of the ibc client.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryClientParamsResponse.</returns>
    public async Task<QueryClientParamsResponse> ClientParams(QueryClientParamsRequest request)
    {
        var result = await client.ClientParamsAsync(request);
        return result;
    }

    /// <summary>
    /// UpgradedClientState queries an Upgraded IBC light client.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryUpgradedClientStateResponse.</returns>
    public async Task<QueryUpgradedClientStateResponse> UpgradedClientState(QueryUpgradedClientStateRequest request)
    {
        var result = await client.UpgradedClientStateAsync(request);
        return result;
    }

    /// <summary>
    /// UpgradedConsensusState queries an Upgraded IBC consensus state.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryUpgradedConsensusStateResponse.</returns>
    public async Task<QueryUpgradedConsensusStateResponse> UpgradedConsensusState(QueryUpgradedConsensusStateRequest request)
    {
        var result = await client.UpgradedConsensusStateAsync(request);
        return result;
    }
}
