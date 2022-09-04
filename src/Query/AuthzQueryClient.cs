using Cosmos.Authz.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class AuthzQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class AuthzQueryClient : GprcBase
{
    private Cosmos.Authz.V1Beta1.Query.QueryClient? _queryClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthzQueryClient"/> class.
    /// </summary>
    /// <param name="secretNetworkClient">The secret network client.</param>
    internal AuthzQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Authz.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Authz.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Authz.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Returns list of `Authorization`, granted to the grantee by the granter.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryGrantsResponse.</returns>
    public async Task<QueryGrantsResponse> Grants(QueryGrantsRequest request)
    {
        var result = await client.GrantsAsync(request);
        return result;
    }

    /// <summary>
    /// GranterGrants returns list of `GrantAuthorization`, granted by granter.
    ///
    /// Since: cosmos-sdk 0.45.2
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryGranterGrantsResponse.</returns>
    public async Task<QueryGranterGrantsResponse> GranterGrants(QueryGranterGrantsRequest request)
    {
        var result = await client.GranterGrantsAsync(request);
        return result;
    }

    /// <summary>
    /// GranteeGrants returns a list of `GrantAuthorization` by grantee.
    ///
    /// Since: cosmos-sdk 0.45.2
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryGranteeGrantsResponse.</returns>
    public async Task<QueryGranteeGrantsResponse> GranteeGrants(QueryGranteeGrantsRequest request)
    {
        var result = await client.GranteeGrantsAsync(request);
        return result;
    }
}
