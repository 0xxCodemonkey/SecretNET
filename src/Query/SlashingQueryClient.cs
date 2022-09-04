using Cosmos.Slashing.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class SlashingQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class SlashingQueryClient : GprcBase
{
    private Cosmos.Slashing.V1Beta1.Query.QueryClient? _queryClient;

    internal SlashingQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Slashing.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Slashing.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Slashing.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Params queries the parameters of slashing module.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryParamsResponse.</returns>
    public async Task<QueryParamsResponse> Params(QueryParamsRequest request)
    {
        var result = await client.ParamsAsync(request);
        return result;
    }

    /// <summary>
    /// SigningInfo queries the signing info of given cons address.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QuerySigningInfoResponse.</returns>
    public async Task<QuerySigningInfoResponse> SigningInfo(QuerySigningInfoRequest request)
    {
        var result = await client.SigningInfoAsync(request);
        return result;
    }

    /// <summary>
    /// SigningInfos queries signing info of all validators.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QuerySigningInfosResponse.</returns>
    public async Task<QuerySigningInfosResponse> SigningInfos(QuerySigningInfosRequest request)
    {
        var result = await client.SigningInfosAsync(request);
        return result;
    }
}
