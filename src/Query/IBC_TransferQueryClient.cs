using Ibc.Applications.Transfer.V1;

namespace SecretNET.Query;

/// <summary>
/// Class IbcTransferQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class IbcTransferQueryClient : GprcBase
{
    private Ibc.Applications.Transfer.V1.Query.QueryClient? _queryClient;

    internal IbcTransferQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Ibc.Applications.Transfer.V1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Ibc.Applications.Transfer.V1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Ibc.Applications.Transfer.V1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// DenomTrace queries a denomination trace information.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDenomTraceResponse.</returns>
    public async Task<QueryDenomTraceResponse> DenomTrace(QueryDenomTraceRequest request)
    {
        var result = await client.DenomTraceAsync(request);
        return result;
    }

    /// <summary>
    /// DenomTraces queries all denomination traces.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDenomTracesResponse.</returns>
    public async Task<QueryDenomTracesResponse> DenomTraces(QueryDenomTracesRequest request)
    {
        var result = await client.DenomTracesAsync(request);
        return result;
    }

    /// <summary>
    /// Params queries all parameters of the ibc-transfer module.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryParamsResponse.</returns>
    public async Task<QueryParamsResponse> Params(QueryParamsRequest request)
    {
        var result = await client.ParamsAsync(request);
        return result;
    }

    /// <summary>
    /// DenomHash queries a denomination hash information.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDenomHashResponse.</returns>
    public async Task<QueryDenomHashResponse> DenomHash(QueryDenomHashRequest request)
    {
        var result = await client.DenomHashAsync(request);
        return result;
    }
}
