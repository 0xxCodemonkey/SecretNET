using Ibc.Core.Channel.V1;

namespace SecretNET.Query;

/// <summary>
/// Class IbcChannelQueryClient.
/// Implements the <see cref="SecretNET.Common.GprcBase" />
/// </summary>
/// <seealso cref="SecretNET.Common.GprcBase" />
public class IbcChannelQueryClient : GprcBase
{
    private Ibc.Core.Channel.V1.Query.QueryClient? _queryClient;

    internal IbcChannelQueryClient(ISecretNetworkClient secretNetworkClient, GrpcChannel grpcChannel, CallInvoker? grpcMessageInterceptor) : base(secretNetworkClient, grpcChannel, grpcMessageInterceptor)
    {
    }

    private Ibc.Core.Channel.V1.Query.QueryClient client
    {
        get
        {
            if (_queryClient == null)
            {
                _queryClient = grpcMessageInterceptor != null ? 
                    new Ibc.Core.Channel.V1.Query.QueryClient(grpcMessageInterceptor) : 
                    new Ibc.Core.Channel.V1.Query.QueryClient(grpcChannel);
            }
            return _queryClient;
        }
    }

    /// <summary>
    /// Channel queries an IBC Channel.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryChannelResponse.</returns>
    public async Task<QueryChannelResponse> Channel(QueryChannelRequest request)
    {
        var result = await client.ChannelAsync(request);
        return result;
    }

    /// <summary>
    /// Channels queries all the IBC channels of a chain.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryChannelsResponse.</returns>
    public async Task<QueryChannelsResponse> Channels(QueryChannelsRequest request)
    {
        var result = await client.ChannelsAsync(request);
        return result;
    }

    /// <summary>
    /// ConnectionChannels queries all the channels associated with a connection end.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryConnectionChannelsResponse.</returns>
    public async Task<QueryConnectionChannelsResponse> ConnectionChannels(QueryConnectionChannelsRequest request)
    {
        var result = await client.ConnectionChannelsAsync(request);
        return result;
    }

    /// <summary>
    /// ChannelClientState queries for the client state for the channel associated with the provided channel identifiers.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryChannelClientStateResponse.</returns>
    public async Task<QueryChannelClientStateResponse> ChannelClientState(QueryChannelClientStateRequest request)
    {
        var result = await client.ChannelClientStateAsync(request);
        return result;
    }

    /// <summary>
    /// ChannelConsensusState queries for the consensus state for the channel associated with the provided channel identifiers.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryChannelConsensusStateResponse.</returns>
    public async Task<QueryChannelConsensusStateResponse> ChannelConsensusState(QueryChannelConsensusStateRequest request)
    {
        var result = await client.ChannelConsensusStateAsync(request);
        return result;
    }

    /// <summary>
    /// PacketCommitment queries a stored packet commitment hash.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryPacketCommitmentResponse.</returns>
    public async Task<QueryPacketCommitmentResponse> PacketCommitment(QueryPacketCommitmentRequest request)
    {
        var result = await client.PacketCommitmentAsync(request);
        return result;
    }

    /// <summary>
    /// PacketCommitments returns all the packet commitments hashes associated with a channel.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryPacketCommitmentsResponse.</returns>
    public async Task<QueryPacketCommitmentsResponse> PacketCommitments(QueryPacketCommitmentsRequest request)
    {
        var result = await client.PacketCommitmentsAsync(request);
        return result;
    }

    /// <summary>
    /// PacketReceipt queries if a given packet sequence has been received on the queried chain
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryPacketReceiptResponse.</returns>
    public async Task<QueryPacketReceiptResponse> PacketReceipt(QueryPacketReceiptRequest request)
    {
        var result = await client.PacketReceiptAsync(request);
        return result;
    }

    /// <summary>
    /// PacketAcknowledgement queries a stored packet acknowledgement hash.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryPacketAcknowledgementResponse.</returns>
    public async Task<QueryPacketAcknowledgementResponse> PacketAcknowledgement(QueryPacketAcknowledgementRequest request)
    {
        var result = await client.PacketAcknowledgementAsync(request);
        return result;
    }

    /// <summary>
    /// PacketAcknowledgements returns all the packet acknowledgements associated with a channel.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryPacketAcknowledgementsResponse.</returns>
    public async Task<QueryPacketAcknowledgementsResponse> PacketAcknowledgements(QueryPacketAcknowledgementsRequest request)
    {
        var result = await client.PacketAcknowledgementsAsync(request);
        return result;
    }

    /// <summary>
    /// UnreceivedPackets returns all the unreceived IBC packets associated with a channel and sequences.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryUnreceivedPacketsResponse.</returns>
    public async Task<QueryUnreceivedPacketsResponse> UnreceivedPackets(QueryUnreceivedPacketsRequest request)
    {
        var result = await client.UnreceivedPacketsAsync(request);
        return result;
    }

    /// <summary>
    /// UnreceivedAcks returns all the unreceived IBC acknowledgements associated with a channel and sequences.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryUnreceivedAcksResponse.</returns>
    public async Task<QueryUnreceivedAcksResponse> UnreceivedAcks(QueryUnreceivedAcksRequest request)
    {
        var result = await client.UnreceivedAcksAsync(request);
        return result;
    }

    /// <summary>
    /// NextSequenceReceive returns the next receive sequence for a given channel.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>QueryNextSequenceReceiveResponse.</returns>
    public async Task<QueryNextSequenceReceiveResponse> NextSequenceReceive(QueryNextSequenceReceiveRequest request)
    {
        var result = await client.NextSequenceReceiveAsync(request);
        return result;
    }
}
