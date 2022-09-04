using Cosmos.Staking.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class StakingQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class StakingQueryClient : GprcBase
{
    private Cosmos.Staking.V1Beta1.Query.QueryClient? _queryClient;

    internal StakingQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Staking.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Staking.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Staking.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Validator queries validator info for given validator address.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryValidatorResponse.</returns>
    public async Task<QueryValidatorResponse> Validator(QueryValidatorRequest request)
    {
        var result = await client.ValidatorAsync(request);
        return result;
    }

    /// <summary>
    /// Validators queries all validators that match the given status.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryValidatorsResponse.</returns>
    public async Task<QueryValidatorsResponse> Validators(QueryValidatorsRequest request)
    {
        var result = await client.ValidatorsAsync(request);
        return result;
    }

    /// <summary>
    /// ValidatorDelegations queries delegate info for given validator.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryValidatorDelegationsResponse.</returns>
    public async Task<QueryValidatorDelegationsResponse> ValidatorDelegations(QueryValidatorDelegationsRequest request)
    {
        var result = await client.ValidatorDelegationsAsync(request);
        return result;
    }

    /// <summary>
    /// ValidatorUnbondingDelegations queries unbonding delegations of a validator.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryValidatorUnbondingDelegationsResponse.</returns>
    public async Task<QueryValidatorUnbondingDelegationsResponse> ValidatorUnbondingDelegations(QueryValidatorUnbondingDelegationsRequest request)
    {
        var result = await client.ValidatorUnbondingDelegationsAsync(request);
        return result;
    }

    /// <summary>
    /// Delegation queries delegate info for given validator delegator pair.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDelegationResponse.</returns>
    public async Task<QueryDelegationResponse> Delegation(QueryDelegationRequest request)
    {
        var result = await client.DelegationAsync(request);
        return result;
    }

    /// <summary>
    /// UnbondingDelegation queries unbonding info for given validator delegator pair.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryUnbondingDelegationResponse.</returns>
    public async Task<QueryUnbondingDelegationResponse> UnbondingDelegation(QueryUnbondingDelegationRequest request)
    {
        var result = await client.UnbondingDelegationAsync(request);
        return result;
    }

    /// <summary>
    /// DelegatorDelegations queries all delegations of a given delegator address.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDelegatorDelegationsResponse.</returns>
    public async Task<QueryDelegatorDelegationsResponse> DelegatorDelegations(QueryDelegatorDelegationsRequest request)
    {
        var result = await client.DelegatorDelegationsAsync(request);
        return result;
    }

    /// <summary>
    /// DelegatorUnbondingDelegations queries all unbonding delegations of a given delegator address.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDelegatorUnbondingDelegationsResponse.</returns>
    public async Task<QueryDelegatorUnbondingDelegationsResponse> DelegatorUnbondingDelegations(QueryDelegatorUnbondingDelegationsRequest request)
    {
        var result = await client.DelegatorUnbondingDelegationsAsync(request);
        return result;
    }

    /// <summary>
    /// Redelegations queries redelegations of given address.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryRedelegationsResponse.</returns>
    public async Task<QueryRedelegationsResponse> Redelegations(QueryRedelegationsRequest request)
    {
        var result = await client.RedelegationsAsync(request);
        return result;
    }

    /// <summary>
    /// DelegatorValidator queries validator info for given delegator validator pair.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDelegatorValidatorResponse.</returns>
    public async Task<QueryDelegatorValidatorResponse> DelegatorValidator(QueryDelegatorValidatorRequest request)
    {
        var result = await client.DelegatorValidatorAsync(request);
        return result;
    }

    /// <summary>
    /// DelegatorValidators queries all validators info for given delegator address.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDelegatorValidatorsResponse.</returns>
    public async Task<QueryDelegatorValidatorsResponse> DelegatorsValidators(QueryDelegatorValidatorsRequest request)
    {
        var result = await client.DelegatorValidatorsAsync(request);
        return result;
    }

    /// <summary>
    /// HistoricalInfo queries the historical info for given height.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryHistoricalInfoResponse.</returns>
    public async Task<QueryHistoricalInfoResponse> HistoricalInfo(QueryHistoricalInfoRequest request)
    {
        var result = await client.HistoricalInfoAsync(request);
        return result;
    }

    /// <summary>
    /// Pool queries the pool info.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryPoolResponse.</returns>
    public async Task<QueryPoolResponse> Pool(QueryPoolRequest request)
    {
        var result = await client.PoolAsync(request);
        return result;
    }

    /// <summary>
    /// Parameters queries the staking parameters.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryParamsResponse.</returns>
    public async Task<QueryParamsResponse> Params(QueryParamsRequest request)
    {
        var result = await client.ParamsAsync(request);
        return result;
    }
    //TODO: All Cosmos.Staking queries
    // Validators
    // Validator
    // ValidatorDelegations
    // ValidatorUnbondingDelegations
    // Delegation
    // UnbondingDelegation
    // DelegatorDelegations
    // DelegatorUnbondingDelegations
    // Redelegations
    // DelegatorValidators
    // DelegatorValidator
    // HistoricalInfo
    // Pool
    // Params

}
