using Cosmos.Feegrant.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class FeegrantQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class FeegrantQueryClient : GprcBase
{
    private Cosmos.Feegrant.V1Beta1.Query.QueryClient? _queryClient;

    internal FeegrantQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Feegrant.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Feegrant.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Feegrant.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Allowance returns fee granted to the grantee by the granter.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryAllowanceResponse.</returns>
    public async Task<QueryAllowanceResponse> Allowance(QueryAllowanceRequest request)
    {
        var result = await client.AllowanceAsync(request);
        return result;
    }

    /// <summary>
    /// Allowances returns all the grants for address.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryAllowancesResponse.</returns>
    public async Task<QueryAllowancesResponse> Allowances(QueryAllowancesRequest request)
    {
        var result = await client.AllowancesAsync(request);
        return result;
    }
}
