using Cosmos.Upgrade.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class UpgradeQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class UpgradeQueryClient : GprcBase
{
    private Cosmos.Upgrade.V1Beta1.Query.QueryClient? _queryClient;

    internal UpgradeQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Upgrade.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Upgrade.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Upgrade.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// CurrentPlan queries the current upgrade plan.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryCurrentPlanResponse.</returns>
    public async Task<QueryCurrentPlanResponse> GetNodeInfo(QueryCurrentPlanRequest request)
    {
        var result = await client.CurrentPlanAsync(request);
        return result;
    }

    /// <summary>
    /// AppliedPlan queries a previously applied upgrade plan by its name.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryAppliedPlanResponse.</returns>
    public async Task<QueryAppliedPlanResponse> AppliedPlan(QueryAppliedPlanRequest request)
    {
        var result = await client.AppliedPlanAsync(request);
        return result;
    }

    /// <summary>
    /// UpgradedConsensusState queries the consensus state that will serve
    /// as a trusted kernel for the next version of this chain. It will only be
    /// stored at the last height of this chain.
    /// UpgradedConsensusState RPC not supported with legacy querier
    /// This rpc is deprecated now that IBC has its own replacement
    /// (https://github.com/cosmos/ibc-go/blob/2c880a22e9f9cc75f62b527ca94aa75ce1106001/proto/ibc/core/client/v1/query.proto#L54)
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryUpgradedConsensusStateResponse.</returns>
    public async Task<QueryUpgradedConsensusStateResponse> UpgradedConsensusState(QueryUpgradedConsensusStateRequest request)
    {
        var result = await client.UpgradedConsensusStateAsync(request);
        return result;
    }

    /// <summary>
    /// ModuleVersions queries the list of module versions from state.
    ///
    /// Since: cosmos-sdk 0.43
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryModuleVersionsResponse.</returns>
    public async Task<QueryModuleVersionsResponse> ModuleVersions(QueryModuleVersionsRequest request)
    {
        var result = await client.ModuleVersionsAsync(request);
        return result;
    }
}
