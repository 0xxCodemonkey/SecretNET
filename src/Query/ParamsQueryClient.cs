using Cosmos.Params.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class ParamsQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class ParamsQueryClient : GprcBase
{
    private Cosmos.Params.V1Beta1.Query.QueryClient? _queryClient;

    internal ParamsQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Params.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Params.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Params.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Params queries a specific parameter of a module, given its subspace and key.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryParamsResponse.</returns>
    public async Task<QueryParamsResponse> Params(QueryParamsRequest request)
    {
        var result = await client.ParamsAsync(request);
        return result;
    }
}
