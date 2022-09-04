using Ibc.Core.Connection.V1;

namespace SecretNET.Query;

/// <summary>
/// Class IbcConnectionQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class IbcConnectionQueryClient : GprcBase
{
    private Ibc.Core.Connection.V1.Query.QueryClient? _queryClient;

    internal IbcConnectionQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Ibc.Core.Connection.V1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Ibc.Core.Connection.V1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Ibc.Core.Connection.V1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Connection queries an IBC connection end.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryConnectionResponse.</returns>
    public async Task<QueryConnectionResponse> Connection(QueryConnectionRequest request)
    {
        var result = await client.ConnectionAsync(request);
        return result;
    }

    /// <summary>
    /// Connections queries all the IBC connections of a chain.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryConnectionsResponse.</returns>
    public async Task<QueryConnectionsResponse> Connections(QueryConnectionsRequest request)
    {
        var result = await client.ConnectionsAsync(request);
        return result;
    }

    /// <summary>
    /// ClientConnections queries the connection paths associated with a client state.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryClientConnectionsResponse.</returns>
    public async Task<QueryClientConnectionsResponse> ClientConnections(QueryClientConnectionsRequest request)
    {
        var result = await client.ClientConnectionsAsync(request);
        return result;
    }

    /// <summary>
    /// ConnectionClientState queries the client state associated with the connection.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryConnectionClientStateResponse.</returns>
    public async Task<QueryConnectionClientStateResponse> ConnectionClientState(QueryConnectionClientStateRequest request)
    {
        var result = await client.ConnectionClientStateAsync(request);
        return result;
    }

    /// <summary>
    /// ConnectionConsensusState queries the consensus state associated with the connection.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryConnectionConsensusStateResponse.</returns>
    public async Task<QueryConnectionConsensusStateResponse> ConnectionConsensusState(QueryConnectionConsensusStateRequest request)
    {
        var result = await client.ConnectionConsensusStateAsync(request);
        return result;
    }
}
