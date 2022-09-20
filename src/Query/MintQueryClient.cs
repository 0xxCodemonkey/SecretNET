using Cosmos.Mint.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class MintQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class MintQueryClient : GprcBase
{
    private Cosmos.Mint.V1Beta1.Query.QueryClient? _queryClient;

    internal MintQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Mint.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Mint.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Mint.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Params returns the total set of minting parameters.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryParamsResponse.</returns>
    public async Task<QueryParamsResponse> Params(QueryParamsRequest request)
    {
        var result = await client.ParamsAsync(request);
        return result;
    }

    /// <summary>
    /// Inflation returns the current minting inflation value.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryInflationResponse.</returns>
    public async Task<QueryInflationResponse> Inflation(QueryInflationRequest request)
    {
        var result = await client.InflationAsync(request);
        return result;
    }

    /// <summary>
    /// AnnualProvisions current minting annual provisions value.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryAnnualProvisionsResponse.</returns>
    public async Task<QueryAnnualProvisionsResponse> AnnualProvisions(QueryAnnualProvisionsRequest request)
    {
        var result = await client.AnnualProvisionsAsync(request);
        return result;
    }
}
