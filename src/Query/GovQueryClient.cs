using Cosmos.Gov.V1Beta1;

namespace SecretNET.Query;

/// <summary>
/// Class GovQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class GovQueryClient : GprcBase
{
    private Cosmos.Gov.V1Beta1.Query.QueryClient? _queryClient;

    internal GovQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Cosmos.Gov.V1Beta1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Cosmos.Gov.V1Beta1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Cosmos.Gov.V1Beta1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Proposal queries proposal details based on ProposalID.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryProposalResponse.</returns>
    public async Task<QueryProposalResponse> Proposal(QueryProposalRequest request)
    {
        var result = await client.ProposalAsync(request);
        return result;
    }

    /// <summary>
    /// Proposals queries all proposals based on given status.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryProposalsResponse.</returns>
    public async Task<QueryProposalsResponse> Proposals(QueryProposalsRequest request)
    {
        var result = await client.ProposalsAsync(request);
        return result;
    }

    /// <summary>
    /// Vote queries voted information based on proposalID, voterAddr.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryVoteResponse.</returns>
    public async Task<QueryVoteResponse> Vote(QueryVoteRequest request)
    {
        var result = await client.VoteAsync(request);
        return result;
    }

    /// <summary>
    /// Votes queries votes of a given proposal.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryVotesResponse.</returns>
    public async Task<QueryVotesResponse> Votes(QueryVotesRequest request)
    {
        var result = await client.VotesAsync(request);
        return result;
    }

    /// <summary>
    /// Params queries all parameters of the gov module.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryParamsResponse.</returns>
    public async Task<QueryParamsResponse> Params(QueryParamsRequest request)
    {
        var result = await client.ParamsAsync(request);
        return result;
    }

    /// <summary>
    /// Deposit queries single deposit information based proposalID, depositAddr.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDepositResponse.</returns>
    public async Task<QueryDepositResponse> Deposit(QueryDepositRequest request)
    {
        var result = await client.DepositAsync(request);
        return result;
    }

    /// <summary>
    /// Deposits queries all deposits of a single proposal.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryDepositsResponse.</returns>
    public async Task<QueryDepositsResponse> Deposits(QueryDepositsRequest request)
    {
        var result = await client.DepositsAsync(request);
        return result;
    }

    /// <summary>
    /// TallyResult queries the tally of a proposal vote.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryTallyResultResponse.</returns>
    public async Task<QueryTallyResultResponse> TallyResult(QueryTallyResultRequest request)
    {
        var result = await client.TallyResultAsync(request);
        return result;
    }
}
