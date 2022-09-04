using Cosmos.Bank.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class BankQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class BankQueryClient : GprcBase
{
    private Cosmos.Bank.V1Beta1.Query.QueryClient? _queryClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="BankQueryClient"/> class.
    /// </summary>
    /// <param name="secretNetworkClient">The secret network client.</param>
    internal BankQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Bank.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Bank.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Bank.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Balance queries the balance of a single coin for a single account.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryBalanceResponse.</returns>
    public async Task<QueryBalanceResponse> Balance(QueryBalanceRequest request)
    {
        var result = await client.BalanceAsync(request);
        return result;
    }

    /// <summary>
    /// Balance queries the balance of a single coin for a single account.
    /// </summary>
    /// <param name="address">The account address.</param>
    /// <param name="denom">The denom.</param>
    /// <returns>Coin.</returns>
    public async Task<Coin> Balance(string address, string denom = "uscrt")
    {
        var result = await client.BalanceAsync(new Cosmos.Bank.V1Beta1.QueryBalanceRequest { Address = address, Denom = denom });
        return result?.Balance;
    }

    /// <summary>
    /// AllBalances queries the balance of all coins for a single account.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryAllBalancesResponse.</returns>
    public async Task<QueryAllBalancesResponse> Balance(QueryAllBalancesRequest request)
    {
        var result = await client.AllBalancesAsync(request);
        return result;
    }

    /// <summary>
    /// SpendableBalances queries the spenable balance of all coins for a single.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QuerySpendableBalancesResponse.</returns>
    public async Task<QuerySpendableBalancesResponse> SpendableBalances(QuerySpendableBalancesRequest request)
    {
        var result = await client.SpendableBalancesAsync(request);
        return result;
    }

    /// <summary>
    /// TotalSupply queries the total supply of all coins.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryTotalSupplyResponse.</returns>
    public async Task<QueryTotalSupplyResponse> TotalSupply(QueryTotalSupplyRequest request)
    {
        var result = await client.TotalSupplyAsync(request);
        return result;
    }

    /// <summary>
    /// SupplyOf queries the supply of a single coin.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QuerySupplyOfResponse.</returns>
    public async Task<QuerySupplyOfResponse> SupplyOf(QuerySupplyOfRequest request)
    {
        var result = await client.SupplyOfAsync(request);
        return result;
    }

    /// <summary>
    /// Params queries the parameters of x/bank module.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryParamsResponse.</returns>
    public async Task<QueryParamsResponse> Params(QueryParamsRequest request)
    {
        var result = await client.ParamsAsync(request);
        return result;
    }

    /// <summary>
    /// DenomsMetadata queries the client metadata of a given coin denomination.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDenomMetadataResponse.</returns>
    public async Task<QueryDenomMetadataResponse> DenomMetadata(QueryDenomMetadataRequest request)
    {
        var result = await client.DenomMetadataAsync(request);
        return result;
    }

    /// <summary>
    /// DenomsMetadata queries the client metadata for all registered coin denominations.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDenomsMetadataResponse.</returns>
    public async Task<QueryDenomsMetadataResponse> DenomsMetadata(QueryDenomsMetadataRequest request)
    {
        var result = await client.DenomsMetadataAsync(request);
        return result;
    }

}
