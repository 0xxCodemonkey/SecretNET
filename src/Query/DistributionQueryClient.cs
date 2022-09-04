using Cosmos.Distribution.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class DistributionQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class DistributionQueryClient : GprcBase
{
    private Cosmos.Distribution.V1Beta1.Query.QueryClient? _queryClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DistributionQueryClient"/> class.
    /// </summary>
    /// <param name="secretNetworkClient">The secret network client.</param>
    internal DistributionQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Distribution.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Distribution.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Distribution.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Params queries params of the distribution module.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryParamsResponse.</returns>
    public async Task<QueryParamsResponse> Balance(QueryParamsRequest request)
    {
        var result = await client.ParamsAsync(request);
        return result;
    }

    /// <summary>
    /// ValidatorOutstandingRewards queries rewards of a validator address.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryValidatorOutstandingRewardsResponse.</returns>
    public async Task<QueryValidatorOutstandingRewardsResponse> ValidatorOutstandingRewards(QueryValidatorOutstandingRewardsRequest request)
    {
        var result = await client.ValidatorOutstandingRewardsAsync(request);
        return result;
    }

    /// <summary>
    /// ValidatorCommission queries accumulated commission for a validator.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryValidatorCommissionResponse.</returns>
    public async Task<QueryValidatorCommissionResponse> ValidatorCommission(QueryValidatorCommissionRequest request)
    {
        var result = await client.ValidatorCommissionAsync(request);
        return result;
    }

    /// <summary>
    /// ValidatorSlashes queries slash events of a validator.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryValidatorSlashesResponse.</returns>
    public async Task<QueryValidatorSlashesResponse> ValidatorSlashes(QueryValidatorSlashesRequest request)
    {
        var result = await client.ValidatorSlashesAsync(request);
        return result;
    }

    /// <summary>
    /// DelegationRewards queries the total rewards accrued by a delegation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDelegationRewardsResponse.</returns>
    public async Task<QueryDelegationRewardsResponse> DelegationRewards(QueryDelegationRewardsRequest request)
    {
        var result = await client.DelegationRewardsAsync(request);
        return result;
    }

    /// <summary>
    /// DelegationTotalRewards queries the total rewards accrued by a each.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDelegationTotalRewardsResponse.</returns>
    public async Task<QueryDelegationTotalRewardsResponse> DelegationTotalRewards(QueryDelegationTotalRewardsRequest request)
    {
        var result = await client.DelegationTotalRewardsAsync(request);
        return result;
    }

    /// <summary>
    /// DelegatorValidators queries the validators of a delegator.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDelegatorValidatorsResponse.</returns>
    public async Task<QueryDelegatorValidatorsResponse> DelegatorValidators(QueryDelegatorValidatorsRequest request)
    {
        var result = await client.DelegatorValidatorsAsync(request);
        return result;
    }

    /// <summary>
    /// DelegatorWithdrawAddress queries withdraw address of a delegator.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDelegatorWithdrawAddressResponse.</returns>
    public async Task<QueryDelegatorWithdrawAddressResponse> DelegatorWithdrawAddress(QueryDelegatorWithdrawAddressRequest request)
    {
        var result = await client.DelegatorWithdrawAddressAsync(request);
        return result;
    }

    /// <summary>
    /// CommunityPool queries the community pool coins.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryCommunityPoolResponse.</returns>
    public async Task<QueryCommunityPoolResponse> CommunityPool(QueryCommunityPoolRequest request)
    {
        var result = await client.CommunityPoolAsync(request);
        return result;
    }

    /// <summary>
    /// FoundationTax queries.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryFoundationTaxResponse.</returns>
    public async Task<QueryFoundationTaxResponse> FoundationTax(QueryFoundationTaxRequest request)
    {
        var result = await client.FoundationTaxAsync(request);
        return result;
    }

}
