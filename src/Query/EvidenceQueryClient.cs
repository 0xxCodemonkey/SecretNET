using Cosmos.Evidence.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class EvidenceQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class EvidenceQueryClient : GprcBase
{
    private Cosmos.Evidence.V1Beta1.Query.QueryClient? _queryClient;

    internal EvidenceQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Evidence.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Evidence.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Evidence.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Evidence queries evidence based on evidence hash.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryEvidenceResponse.</returns>
    public async Task<QueryEvidenceResponse> FoundationTax(QueryEvidenceRequest request)
    {
        var result = await client.EvidenceAsync(request);
        return result;
    }

    /// <summary>
    /// AllEvidence queries all evidence.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryAllEvidenceResponse.</returns>
    public async Task<QueryAllEvidenceResponse> AllEvidence(QueryAllEvidenceRequest request)
    {
        var result = await client.AllEvidenceAsync(request);
        return result;
    }
}
